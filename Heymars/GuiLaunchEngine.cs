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

namespace GuiLaunch
{
    public interface IProcessEvents
    {
        void ProcessStatusChanged(int index, string status);
        void SpeakStatus(int index, string message);
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

    public class CommandEntry
    {
        public string c { get; set; }
        public string cwd { get; set; }

        public string title { get; set; }
        public bool? shell { get; set; }
        public override string ToString()
        {
            return c + (cwd == null ? "" : " || Cwd: " + cwd);  
        }

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
            var o = await Cli.Wrap("jsonnet").WithArguments(fname).ExecuteBufferedAsync();
            var json = o.StandardOutput;
            AnsiConsole.Write(new Markup($"Jsonnet expansion:\n [blue]{json.EscapeMarkup()}[/]"));
            return JsonSerializer.Deserialize<ConfigFile>(o.StandardOutput);


        }

        public async Task Read(string fname)
        {

            Cwd = Path.GetDirectoryName(fname);
            ConfigFile configFile = null;
            if (fname.EndsWith(".json"))
            {
                configFile = ReadJsonFile(fname);
            } else if (fname.EndsWith(".jsonnet"))
            {
                configFile = await ReadJsonnetFile(fname);
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

            Cwd = Path.GetFullPath(Cwd);
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
            var commandString = command.c;
            var parts = commandString.Split(new char[] { ' ' }, 2);
            if (parts[0] == "cd")
            {
                Cwd = parts[1];
                Console.WriteLine($"Cd to: {Cwd}");
                
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
                var (exit, secs) = await StreamResults(index, index.ToString(), cmd).ConfigureAwait(false);

                ReportProcessExit(index, exit);
            }
            RunningPid.Remove(index); 
            AnsiConsole.Write(new Markup($"[green] === DONE === [/] [blue]{commandString}[/]\n"));
        }

        private async Task<(int ExitCode, int Seconds)> StreamResults(int index, string v, Command cmd)
        {
            var running = Running[index];
            var buf = running.OutputBuf;
            void write(string title, string color, string text)
            {
                if (CollectOutput)
                {
                    buf.PushBack(text);
                }
                AnsiConsole.Write(new Markup($"[{color}]{title}:[/] {text.EscapeMarkup()}\n"));
                LogStream?.WriteLine($"{title}: {text}");

            }

            Color ocolor = index % 14 + 2;
            var cname = ocolor.ToMarkup();
            await foreach (var cmdEvent in cmd.ListenAsync())
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
            _listener.SpeakStatus(index, $"{status} run {index}");
        }

        private async Task OpenFileInEditor(string fname)
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
                default:
                    {
                        supress = false;
                        break;
                    }

            }
            return supress;
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


        public string GetOutput(int index)
        {
            if (!Running.ContainsKey(index))
            {
                return null;
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

        internal void ClearStatuses()
        {
            Running.Clear();
            RefreshAllStatuses();
        }
    }
}
