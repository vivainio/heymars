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
using System.Configuration;

namespace GuiLaunch
{
    public interface IProcessEvents
    {
        void ProcessStatusChanged(int index, string status);
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


        IProcessEvents _listener = null;

        public IProcessEvents Listener { get => _listener; set => _listener = value; }

        public static CommandEntry[] ReadTextFile(string fname)
        {
            var lines = File.ReadAllLines(fname);
            return lines.Select(line => new CommandEntry
            {
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
            if (fname.EndsWith(".txt"))
            {
                Commands = ReadTextFile(fname);
            } else if (fname.EndsWith(".json"))
            {
                configFile = ReadJsonFile(fname);
            } else if (fname.EndsWith(".jsonnet"))
            {
                configFile = await ReadJsonnetFile(fname);
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
                var (exit, secs) = await StreamResults(index, parts[0], cmd);

                ReportProcessExit(index, GetExitDesc(exit, secs));
            }
            RunningPid.Remove(index); 
            AnsiConsole.Write(new Markup($"[green] === DONE === [/] [blue]{commandString}[/]\n"));
        }

        private async Task<(int ExitCode, int Seconds)> StreamResults(int index, string v, Command cmd)
        {
            void write(string title, string color, string text)
            {
                AnsiConsole.Write(new Markup($"[{color}]{title}:[/] {text.EscapeMarkup()}\n"));


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
                Process.GetProcessById(pid).Kill();
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
    }
}
