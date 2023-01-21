using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiLaunch
{
    public class LauncherSettings: CommandSettings
    {
        [Description("Path to config file. Can be .json, .jsonnet or flat text file")]
        [CommandArgument(0, "<CONFIG_FILE>")]
        public string Config { get; set; }
        [Description("Run without GUI. This automatically launches all the commands at the same time")]
        [CommandOption("--headless")]
        public bool Headless { get; set; }
        [Description("When running with gui mode, launch all commands at the same time")]
        [CommandOption("--runall")]
        public bool RunAll { get; set; }

        [Description("List of command ids (or indexes) to run immediately, e.g. 'one,two,12,last")]
        [CommandOption("--run")]
        public string Run { get; set; }
    }


    public class LauncherCommand : AsyncCommand<LauncherSettings>
    {

        public override async Task<int> ExecuteAsync(CommandContext context, LauncherSettings settings)
        {
            var eng = new GuiLaunchEngine();
            await eng.PopulateFromConfigFile(settings.Config);
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
            form.Text = "Heymars " + eng.Cwd;
            Application.Run(form);
            return 0;
      }
    }

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
      
        [STAThread]
        static int Main(string[] args)
        {
           
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            var app = new CommandApp();
            app.SetDefaultCommand<LauncherCommand>();
            app.Configure(config =>
            {
                config.AddCommand<LauncherCommand>("launcher");
            });

            return app.Run(args);

        }
    }
}
