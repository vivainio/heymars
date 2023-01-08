using Spectre.Console.Cli;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuiLaunch
{
    public class LauncherSettings: CommandSettings
    {
        [CommandArgument(0, "[ConfigFile]")]
        public string Config { get; set; }
        [CommandOption("--headless")]
        public bool Headless { get; set; }
        [CommandOption("--runall")]
        public bool RunAll { get; set; }

    }


    public class LauncherCommand : AsyncCommand<LauncherSettings>
    {

        public override async Task<int> ExecuteAsync(CommandContext context, LauncherSettings settings)
        {
            var eng = new GuiLaunchEngine();
            await eng.PopulateFromConfigFile(settings.Config ?? "commands.txt");
            if (settings.Headless)
            {
                await eng.RunAll();
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
