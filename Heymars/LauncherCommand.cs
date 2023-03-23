using Spectre.Console.Cli;
using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiLaunch
{
    public class LauncherCommand : AsyncCommand<LauncherSettings>
    {
        public override async Task<int> ExecuteAsync(CommandContext context, LauncherSettings settings)
        {
            var eng = new GuiLaunchEngine();
            await eng.PopulateFromConfigFile(settings.Config);
            if (settings.Root != null)
            {
                eng.Cwd = Path.GetFullPath(settings.Root);
            }

            if (eng.Commands == null)
            {
                Console.WriteLine("No commands, exiting");
                return 1;
            }
            if (settings.Headless)
            {
                if (settings.Run != null)
                {
                    await eng.RunCommands(settings.Run.Split(","));
                } else
                {
                    await eng.RunAll();
                }
                return 0;
            }
            var form = new Form1(eng, settings);
            if (settings.Show)
            {
                
                var configDto = new ConfigFile
                {
                    commands = eng.Commands.Select(c => c with
                    {
                        index = null
                    }).ToArray(),
                    root = eng.Cwd
                };
                var asText = eng.Jsonize(configDto);
                Console.WriteLine(asText);
            }

            form.Text = "Heymars: " + eng.Cwd;
            Application.Run(form);
            return 0;
      }
    }
}
