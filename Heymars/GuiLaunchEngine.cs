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
        public override string ToString()
        {
            return c + (cwd == null ? "" : " || Cwd: " + cwd);  
        }

    }


    public class GuiLaunchEngine
    {
        public CommandEntry[] Commands = null;
        public string Cwd = null;


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

        public static CommandEntry[] ReadJsonFile(string fname)
        {
            var cont = File.ReadAllBytes(fname);
            return JsonSerializer.Deserialize<CommandEntry[]>(cont);
        }

        public void Read(string fname)
        {
           
            Cwd = Path.GetDirectoryName(fname);
            
            if (fname.EndsWith(".txt"))
            {
                Commands = ReadTextFile(fname);
            } else if (fname.EndsWith(".json"))
            {
                Commands = ReadJsonFile(fname);
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
            Stream stdout = null;
            Stream stderr = null;
            if (outputCallback == null)
            {
                stdout = Console.OpenStandardOutput();
                stderr = Console.OpenStandardError();

            }

            var cwd = !string.IsNullOrEmpty(command.cwd) ? Path.Combine(Cwd, command.cwd) : Cwd;

            var cmd = Cli.Wrap("cmd")
                .WithArguments("/c " + commandString)
                .WithWorkingDirectory(cwd)
                .WithValidation(CommandResultValidation.None);

            _listener.ProcessStatusChanged(index, "...");
            if (outputCallback != null)
            {
                AnsiConsole.Write(new Markup("[blue]" + commandString + "[/]\n"));
                var bufout = await cmd.ExecuteBufferedAsync(Encoding.UTF8);
                ReportProcessExit(index, bufout);
                await outputCallback(bufout.StandardOutput, bufout.StandardError);
            } else
            {
                cmd = cmd
                    .WithStandardOutputPipe(PipeTarget.ToStream(stdout))
                    .WithStandardErrorPipe(PipeTarget.ToStream(stderr));
                var res = await cmd.ExecuteAsync();
                ReportProcessExit(index, res);

            }
            AnsiConsole.Write(new Markup($"[green] === DONE === [/] [blue]{commandString}[/]\n"));
        }

        private void ReportProcessExit(int index, CommandResult res)
        {
            var message = "";
            if (res.ExitCode == 0)
            {
                message = $"ok ";
            }
            else
            {
                message = $"err {res.ExitCode} ";
            }
            message += $"{res.RunTime.TotalSeconds:0}s";
            _listener.ProcessStatusChanged(index, message);

        }

        private async Task OpenInEditor(string content)
        {
            await Cli.Wrap("code").WithArguments("-").WithStandardInputPipe(PipeSource.FromString(content)).ExecuteAsync();

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
