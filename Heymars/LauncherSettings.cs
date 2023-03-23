using Spectre.Console.Cli;
using System.ComponentModel;

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

        [Description("Set root directory for the commands, overrides 'root' in config")]
        [CommandOption("--root")]
        public string Root { get; set; }

        [Description("Show the config in json format (e.g. when starting with text file)")]
        [CommandOption("--show")]
        public bool Show { get; set; }
    }
}
