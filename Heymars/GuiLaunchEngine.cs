using CliWrap;
using CliWrap.Buffered;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spectre.Console;
using System.Text.Json;
using System.Diagnostics;
using CliWrap.EventStream;
using CircularBuffer;
using Timer = System.Threading.Timer;
using System.Threading;

namespace GuiLaunch
{
    public class StoredSettings
    {
        public List<string> history { get; set; }
    }

    public class SettingsStorage
    {
        public string Location => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "heymars", "settings.json"); 
        public void LoadAndModify(Action<StoredSettings> modify)
        {
            StoredSettings storedSettings;
            if (!File.Exists(Location))
            {
                storedSettings = new StoredSettings
                {
                    history = new List<string>()
                };
                Directory.CreateDirectory(Path.GetDirectoryName(Location));
            } else
            {
                var cont = File.ReadAllBytes(Location);

                storedSettings = JsonSerializer.Deserialize<StoredSettings>(cont);
                storedSettings.history ??= new List<string>();

            }
            modify(storedSettings);
            var newCont = JsonSerializer.SerializeToUtf8Bytes(storedSettings);
            File.WriteAllBytes(Location, newCont);
        }
    }
    public interface IProcessEvents
    {
        void ProcessStatusChanged(int index, string status);
        void SpeakStatus(string message);
        void Log(string message);
        void RepaintNeeded();

    }


    public class RunningCommand
    {
        public string Pid { get; set; }
        public CircularBuffer<string> OutputBuf = new(50000);
        public List<string> ErrorLines = new();
        public Stopwatch Started = null;
        public int? ExitCode { get; set; }
        public int StdOutLines { get; set; }
        public int StdErrLines { get; set; }
        public string InternalError { get; set; }
        
    }

    public class CommandStats
    {
        // msec
        public long PrevDuration { get; set; }
    }

    public class ConfigFile
    {
        public CommandEntry[] commands { get; set; }
        public string root { get; set; }
    }

    public class GuiLaunchEngine
    {
        Timer _timer;
        public CommandEntry[] Commands = null;
        CancellationTokenSource _cts = new CancellationTokenSource();
        public string ConfigFilePath { get; private set; }

        public string Cwd = null;

        public Dictionary<int, int> RunningPid = new();

        Dictionary<int, RunningCommand> Running = new();
        Dictionary<int, CommandStats> Stats = new();


        IProcessEvents _listener = null;

        public void StartPolling()
        {
            _timer = new Timer((o) => RefreshAllStatuses(), null, 1000, 1000);
        }
        public GuiLaunchEngine()
        {
        }

        public StreamWriter LogStream { get; set; }

        public IProcessEvents Listener { get => _listener; set => _listener = value; }

        private string NiceTimeText(long milliseconds)
        {
            var t = TimeSpan.FromMilliseconds(milliseconds);
            if (t.Minutes > 0)
                return $"{t.Minutes}min {t.Seconds}s";
            return $"{t.Seconds}s";
        }
        public static CommandEntry[] ReadTextFile(string fname)
        {
            static bool ValidCommand(string s)
            {
                if (string.IsNullOrEmpty(s.Trim()))
                {
                    return false;
                }

                return true;

            }

            var lines = File.ReadAllLines(fname);
            return lines.Where(ValidCommand).Select(line => new CommandEntry
            {
                shell = true,
                c = line

            }).ToArray();

        }

        public static ConfigFile ReadJsonFile(string fname)
        {

            var cont = File.ReadAllBytes(fname);
            return JsonSerializer.Deserialize<ConfigFile>(cont);
        }

        public static async Task<ConfigFile> ReadJsonnetFile(string fname)
        {
            var o = await Cli.Wrap("jsonnet").WithArguments(fname).WithValidation(CommandResultValidation.None).ExecuteBufferedAsync();
            var json = o.StandardOutput;
            AnsiConsole.Write(new Markup($"Jsonnet expansion:\n [blue]{json.EscapeMarkup()}[/]"));
            if (o.ExitCode != 0)
            {
                AnsiConsole.Write(new Markup($"[red]Jsonnet error[/]:\n [blue]{o.StandardError.EscapeMarkup()}[/]"));


                return null;
            }
            return JsonSerializer.Deserialize<ConfigFile>(o.StandardOutput);


        }

        public async Task PopulateFromConfigFile(string fname)
        {
            Cwd = Path.GetDirectoryName(fname);
            ConfigFile configFile = null;
            if (fname.EndsWith(".json"))
            {
                configFile = ReadJsonFile(fname);
            } else if (fname.EndsWith(".jsonnet"))
            {
                configFile = await ReadJsonnetFile(fname);
                if (configFile == null)
                {
                    return;
                }
            }
            else {
                Commands = ReadTextFile(fname);
            }
            if (configFile != null)
            {
                if (configFile.root != null)
                {
                    Cwd = Path.Combine(Cwd, configFile.root);
                }

                Commands = configFile.commands;
            }
            for (int i = 0; i < Commands.Length; i++)
            {
                Commands[i].index = i;
            }
            ConfigFilePath = Path.GetFullPath(fname);
            Cwd = Path.GetFullPath(Cwd);
            _listener?.RepaintNeeded();
        }


        private string CalculateStatusString(int index)
        {
            // never run
            if (!Running.ContainsKey(index))
            {
                return "";
            }
            var running = Running[index];
            if (running == null)
            {
                // never run
                return "";
            }
            if (running.InternalError != null)
            {
                return running.InternalError;
            }
            if (running.ExitCode == 0)
            {
                return "ok";
            }

            var elapsed = running.Started.ElapsedMilliseconds;
            if (running.ExitCode != null && running.ExitCode != 0)
            {
                return $"err {running.ExitCode} {NiceTimeText(elapsed)}";

            }

            // elapsed mode! let's calculate timing etc


            var prefix = $"... {NiceTimeText(elapsed)} e:{running.StdErrLines} o:{running.StdOutLines}";


            var stat = Stats.GetValueOrDefault(index);

            if (stat == null)
                return prefix;

            var progress = stat.PrevDuration > elapsed ? $"ETA: {NiceTimeText(stat.PrevDuration - elapsed)}"
                : $"{(int)((float)elapsed / stat.PrevDuration * 100)}%";
            return $"{prefix} {progress}";
        }


        private void RefreshSingleStatus(int index)
        {
            var s = CalculateStatusString(index);
            _listener?.ProcessStatusChanged(index, s);
        }

        private void RefreshAllStatuses()
        {
            if (Commands == null)
            {
                return;
            }
            for (int i = 0; i < Commands.Length; i++)
            {
                RefreshSingleStatus(i);
            }

        }
        public async Task Selected(int index, Func<string, string, Task> outputCallback = null)
        {
            var command = Commands[index];
            if (command.runtags != null)
            {
                var tagged = FindTaggedCommands(command.runtags);
                foreach (var i in tagged)
                {
                    var t = Selected(i);
                }
            }
            if (command?.c == null)
            {
                // e.g. comments don't have 'c' attribute
                return;

            }
            var commandString = command.c;
            var parts = commandString.Split(new char[] { ' ' }, 2);
            if (parts[0] == "cd")
            {
                var oldCwd = Cwd;
                Cwd = Path.GetFullPath(Path.Combine(Cwd, parts[1]));
                Console.WriteLine($"Cd to: {Cwd} (old was '{oldCwd})");
                return;
            }

            var cwd = !string.IsNullOrEmpty(command.cwd) ? Path.Combine(Cwd, command.cwd) : Cwd;
            AnsiConsole.Write(new Markup($">>> {index}: [blue]" + commandString + "[/]\n"));
            Command cmd = null;
            var absbin = Path.Combine(cwd, parts[0]);
            if (command.shell ?? true)
            {
                cmd = Cli.Wrap("cmd").WithArguments("/c " + commandString);

            } else
            {
                if (!File.Exists(absbin))
                {
                    AnsiConsole.Write(new Markup($"\n[red]File not found: [/][yellow]{absbin}[/]\n"));
                    Running[index] = new RunningCommand
                    {
                        InternalError = "Notfound"
                    };
                    return;
                }

                cmd = Cli.Wrap(absbin);
                if (parts.Length == 2)
                {
                    cmd = cmd.WithArguments(parts[1]);
                }


            }

            cmd = cmd
                .WithWorkingDirectory(cwd)
                .WithValidation(CommandResultValidation.None);

            var running = new RunningCommand();
            Running[index] = running;

            running.Started = Stopwatch.StartNew();

            _listener?.ProcessStatusChanged(index, "...");
            int pid = -1;
            if (outputCallback != null)
            {
                var task = cmd.ExecuteBufferedAsync(Encoding.UTF8);
                pid = task.ProcessId;
                RunningPid[index] = pid;
                var bufout = await task;
                ReportProcessExit(index, bufout.ExitCode);
                await outputCallback(bufout.StandardOutput, bufout.StandardError);
            } else
            {
                var (exit, secs) = await StreamResults(index, command.id ?? index.ToString(), cmd).ConfigureAwait(false);

                ReportProcessExit(index, exit);
            }
            RunningPid.Remove(index);
            AnsiConsole.Write(new Markup($"[green] === DONE === [/] [blue]{commandString}[/]\n"));
        }

        private List<int> FindTaggedCommands(List<string> tags)
        {
            int i = 0;

            var ts = new List<int>();
            foreach (var command in Commands)
            {
                if (command?.tags != null && command.tags.Intersect(tags).Any())
                {

                    ts.Add(i);                    
                }
                i++;
            }
            return ts;
        }

        private void TriggerMatcher(CommandEntry command, Matcher matcher, string line)
        {
            var say = matcher.say;
            if (say != null)
            {
                var toSay = say.Replace("{id}", command.id ?? command.index.ToString());

                _listener.SpeakStatus(toSay);
            }
            if (matcher.log)
            {
                _listener.Log(line);
            }
        }
        private async Task<(int ExitCode, int Seconds)> StreamResults(int index, string v, Command cmd)
        {
            var running = Running[index];
            var buf = running.OutputBuf;
            var command = Commands[index];
            var matchers = command.matchers ?? Enumerable.Empty<Matcher>();
            void write(string title, string color, string text)
            {
                if (CollectOutput)
                {
                    buf.PushBack(text);
                }
               
                AnsiConsole.Write(new Markup($"[{color}]{title}:[/] {text.EscapeMarkup()}\n"));
                LogStream?.WriteLine($"{title}: {text}");
                foreach (var m in matchers)
                {
                    if (Matcher.MatchAny(m, text))
                    {
                        TriggerMatcher(command, m, text);
                        // only one matcher per line
                        break;
                    }
                }
            }

            Color ocolor = index % 14 + 2;
            var cname = ocolor.ToMarkup();
            cmd = cmd.WithStandardInputPipe(PipeSource.FromStream(Console.OpenStandardInput()));
            await foreach (var cmdEvent in cmd.ListenAsync(_cts.Token))
            {
                switch (cmdEvent)
                {
                    case StartedCommandEvent start:
                        write(v, cname, "Start pid " + start.ProcessId);
                        RunningPid[index] = start.ProcessId;
                        break;
                    case StandardOutputCommandEvent stdOut:
                        write(v, cname, stdOut.Text);
                        running.StdOutLines++;
                        break;
                    case StandardErrorCommandEvent stdErr:
                        write(v, "red", stdErr.Text);
                        if (stdErr.Text.Length > 0)
                        {
                            running.StdErrLines++;
                            running.ErrorLines.Add(stdErr.Text);
                        }
                        break;
                    case ExitedCommandEvent ex:
                        return (ex.ExitCode, 0);
                    default:
                        write("UNK", "red", "Unknown event :" + cmdEvent.ToString());
                        break;
                }
            }
            return (0, 0);

        }

        private void ReportProcessExit(int index, int exitCode)
        {
            var running = Running.GetValueOrDefault(index);
            if (running == null)
                return;
            running.ExitCode = exitCode;
            running.Started.Stop();
            Stats[index] = new CommandStats
            {
                PrevDuration = running.Started.ElapsedMilliseconds
            };

            RefreshSingleStatus(index);
            var status = exitCode == 0 ? "completed" : "failed";
            _listener.SpeakStatus($"{status} run {index}");
        }

        public async Task OpenFileInEditor(string fname)
        {
            await Cli.Wrap("code").WithArguments(fname).ExecuteAsync();
        }
        private async Task OpenInEditor(string content)
        {
            await Cli.Wrap("code").WithArguments("-").WithStandardInputPipe(PipeSource.FromString(content)).ExecuteAsync();

        }

        private void KillAtIndex(int index)
        {
            if (RunningPid.ContainsKey(index))
            {
                int pid = RunningPid[index];
                AnsiConsole.Write(new Markup("[red]Killing process[/]"));
                try
                {
                    Process.GetProcessById(pid).Kill(true);
                    ReportProcessExit(index, -999);
                } catch (ArgumentException)
                {
                    _listener.ProcessStatusChanged(index, "err?");
                }

            }

            var command = Commands[index];
            if (command.runtags != null)
            {
                var indexes = FindTaggedCommands(command.runtags);
                foreach(var i in indexes)
                {
                    KillAtIndex(i);
                }

            }


        }
        internal async Task<bool> KeyPress(Keys keyChar, int index)
        {

            var supress = true;
            switch (keyChar)
            {
                case Keys.Enter:
                    {
                        await Selected(index).ConfigureAwait(false);
                        break;
                    }
                case Keys.Space:
                    {
                        await Selected(index, async (o, e) =>
                        {
                            var err = string.IsNullOrEmpty(e) ? "" : "\n...\nstderr:\n" + e;
                            await OpenInEditor(o + err);
                        }).ConfigureAwait(false);
                        break;
                    }
                case Keys.Back:
                    {
                        KillAtIndex(index);
                        break;
                    }
                case Keys.F5:
                    {
                        await PopulateFromConfigFile(ConfigFilePath);
                        break;
                    }
                default:
                    {
                        supress = false;
                        break;
                    }

            }
            return supress;
        }

        private void Refresh()
        {
            throw new NotImplementedException();
        }

        string LogFileName => Path.Combine(Cwd, "heymars.log");

        public bool CollectOutput { get; internal set; }

        internal void StartLog()
        {
            LogStream = new StreamWriter(LogFileName);
            LogStream.AutoFlush = true;
        }
        internal async void StopLog()
        {
            var s = LogStream;
            LogStream = null;
            s.Flush();
            s.Close();
            await OpenFileInEditor(LogFileName);
            LogStream = null;
        }

        public async Task RunAll()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < Commands.Length; i++)
            {
                tasks.Add(Selected(i, null));
            }
            await Task.WhenAll(tasks);
        }

        public async Task RunCommands(IReadOnlyList<string> commandids)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < Commands.Length; i++)
            {
                var tomatch = Commands[i].id ?? i.ToString();
                if (commandids.Contains(tomatch))
                {
                    tasks.Add(Selected(i, null));
                }
            }
            await Task.WhenAll(tasks);
        }


        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            
        };

        public string GetLabel(int index)
        {
            var command = Commands[index];
            if (command.c != null)
            {
                return command.c;
            } 
            if (command.runtags != null)
            {
                return "Run tags: " + string.Join(", ", command.runtags);
            }
            return command.c ?? "no c";
        }
        public string GetOutput(int index)
        {
            if (!Running.ContainsKey(index))
            {
                var command = Commands[index];
                return JsonizeCommand(command);
            }

            var running = Running[index];
            var buf = running.OutputBuf;
           
            
            var sb = new StringBuilder();
            // start with errors...

            if (running.ErrorLines.Count > 0)
            {
                sb.AppendLine("// ==== Stderr lines from whole run start ====");
                foreach (var line in running.ErrorLines)
                {
                    if (line.Trim().Length > 0)
                        sb.AppendLine(line);
                }
                sb.AppendLine("// ==== Stderr lines end, full output starts ====");
            }

            var segs = buf.ToArraySegments();
            foreach (var seg in segs )
            {                
                sb.Append(string.Join("\r\n", seg));
                sb.AppendLine("\r\n");
            }
            return sb.ToString();
        }

        private string JsonizeCommand(CommandEntry command)
        {
            return JsonSerializer.Serialize(command, jsonOptions);
        }

        internal void ClearStatuses()
        {
            Running.Clear();
            RefreshAllStatuses();
        }

        private void StyleRow(CommandEntry command, DataGridViewCellStyle defaultCellStyle)
        {
            if (command.fgcolor != null)
            {
                defaultCellStyle.ForeColor = System.Drawing.Color.FromName(command.fgcolor);

            }
            if (command.bgcolor != null) 
            {
                defaultCellStyle.BackColor = System.Drawing.Color.FromName(command.bgcolor);

            }


        }
        public void DrawGrid(DataGridView commandGrid)
        {
            commandGrid.RowPrePaint
                += (sender, e) =>
                {
                    var command = Commands[e.RowIndex];
                    StyleRow(command, commandGrid.Rows[e.RowIndex].DefaultCellStyle);
                };
            commandGrid.Rows.Clear();
            for (int index = 0; index < Commands.Count(); index++)
            {
                var c = Commands[index];
                commandGrid.Rows.Add(new[] { c.id ?? (object)index, (string)c.title ?? c.c, "" });

            }
        }

        internal void PopulateFileList(ComboBox cbCurrentConfig)
        {
            var storage = new SettingsStorage();
            storage.LoadAndModify((settings) =>
            {
                // move to first
                settings.history.Remove(ConfigFilePath);
                settings.history.Insert(0, ConfigFilePath);
                cbCurrentConfig.Items.AddRange(settings.history.ToArray());
            });
            cbCurrentConfig.SelectedIndex = 0;
        }
    }
}
