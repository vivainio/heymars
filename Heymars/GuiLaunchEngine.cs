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
using Microsoft.VisualBasic.Logging;
using CircularBuffer;
using System.Reflection.Metadata;

namespace GuiLaunch
{
    public interface IProcessEvents
    {
        void ProcessStatusChanged(int index, string status);
    }

    public class RunningCommand
    {
        public string Pid { get; set; }
        public CircularBuffer<string> OutputBuf = new CircularBuffer<string>(50000); 
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
        public CommandEntry[] Commands = null;
        public string Cwd = null;

        public Dictionary<int, int> RunningPid = new Dictionary<int, int>();
        Dictionary<int, RunningCommand> Running = new Dictionary<int, RunningCommand>();


        IProcessEvents _listener = null;
        public StreamWriter LogStream { get; set; }

        public IProcessEvents Listener { get => _listener; set => _listener = value; }

        public static CommandEntry[] ReadTextFile(string fname)
        {
            bool ValidCommand(string s)
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
                shell= true,
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
            AnsiConsole.Write(new Markup(">>> [blue]" + commandString + "[/]\n"));
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
                    return;
                }
               
                cmd = Cli.Wrap(absbin).WithArguments(parts[1]);
                

            }

            cmd = cmd
                .WithWorkingDirectory(cwd)
                .WithValidation(CommandResultValidation.None);

            Running[index] = new RunningCommand();

            _listener.ProcessStatusChanged(index, "...");
            int pid = -1;
            if (outputCallback != null)
            {
                var task = cmd.ExecuteBufferedAsync(Encoding.UTF8);
                pid = task.ProcessId;
                RunningPid[index] = pid;
                var bufout = await task;
                ReportProcessExit(index, GetExitDesc(bufout.ExitCode, (int)bufout.RunTime.TotalSeconds));
                await outputCallback(bufout.StandardOutput, bufout.StandardError);
            } else
            {
                var (exit, secs) = await StreamResults(index, index.ToString(), cmd);

                ReportProcessExit(index, GetExitDesc(exit, secs));
            }
            RunningPid.Remove(index); 
            AnsiConsole.Write(new Markup($"[green] === DONE === [/] [blue]{commandString}[/]\n"));
        }

        private async Task<(int ExitCode, int Seconds)> StreamResults(int index, string v, Command cmd)
        {


            var buf = Running[index].OutputBuf;
            void write(string title, string color, string text)
            {
                if (CollectOutput)
                {
                    buf.PushBack(text);
                }
                AnsiConsole.Write(new Markup($"[{color}]{title}:[/] {text.EscapeMarkup()}\n"));
                if (LogStream != null)
                {
                    LogStream.WriteLine($"{title}: {text}");
                    
                }

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
                        break;
                    case StandardErrorCommandEvent stdErr:
                        write(v, "red", stdErr.Text); 
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

        private string GetExitDesc(int exitCode, int secs)
        {
            var message = "";

            if (exitCode == 0)
            {
                message = $"ok ";
            }
            else
            {
                message = $"err {exitCode} ";
            }
            if (secs > 0)
            {
                message += $"{secs}s";

            }
            return message;

        }
        private void ReportProcessExit(int index, string message)
        {
            _listener.ProcessStatusChanged(index, message);

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
                        await Selected(index);
                        break;
                    }
                case Keys.Space:
                    {
                        await Selected(index, async (o, e) =>
                        {
                            var err = string.IsNullOrEmpty(e) ? "" : "\n...\nstderr:\n" +e;
                            await OpenInEditor(o + err);
                        });
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

        public string GetOutput(int index)
        {
            if (!Running.ContainsKey(index))
            {
                return null;
            }
            var buf = Running[index]?.OutputBuf;
           
            var sb = new StringBuilder();
            var segs = buf.ToArraySegments();
            foreach (var seg in segs )
            {                
                sb.Append(string.Join("\r\n", seg));
                sb.AppendLine("\r\n");
            }
            return sb.ToString();
        }

    }
}
