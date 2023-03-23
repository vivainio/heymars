using Spectre.Console.Cli;
using System;
using System.Windows.Forms;

namespace GuiLaunch
{

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
