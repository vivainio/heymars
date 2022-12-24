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

    }

    public class LauncherCommand : Command<LauncherSettings>
    {
        public override int Execute(CommandContext context, LauncherSettings settings)
        {
            var eng = new GuiLaunchEngine();
            eng.Read(settings.Config ?? "commands.txt");
            Application.Run(new Form1(eng));
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
