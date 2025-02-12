using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Spectre.Console;

namespace GuiLaunch
{
    internal static class ConfigReaders
    {
        public static ConfigFile ReadJsonFile(string fname)
        {
            var cont = File.ReadAllBytes(fname);
            return JsonSerializer.Deserialize<ConfigFile>(cont);
        }

        public static async Task<ConfigFile> ReadJsonnetFile(string fname)
        {
            var o = await Cli.Wrap("jsonnet")
                .WithArguments(fname)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();
            var json = o.StandardOutput;
            //AnsiConsole.Write(new Markup($"Jsonnet expansion:\n [blue]{json.EscapeMarkup()}[/]"));
            if (o.ExitCode != 0)
            {
                AnsiConsole.Write(
                    new Markup(
                        $"[red]Jsonnet error[/]:\n [blue]{o.StandardError.EscapeMarkup()}[/]"
                    )
                );

                return null;
            }
            return JsonSerializer.Deserialize<ConfigFile>(o.StandardOutput);
        }

        public static CommandEntry[] ReadTextFile(string fname)
        {
            static CommandEntry CreateCommand(string s)
            {
                if (string.IsNullOrEmpty(s.Trim()))
                {
                    return null;
                }

                // you can interleave some commands in one line if you want
                if (s.StartsWith("{"))
                {
                    return JsonSerializer.Deserialize<CommandEntry>(s);
                }

                if (s.StartsWith("#") || s.StartsWith("//") || s.ToLower().StartsWith("rem "))
                {
                    return new CommandEntry
                    {
                        fgcolor = "blue",
                        id = "",
                        title = s,
                    };
                }
                return new CommandEntry { shell = true, c = s };
            }

            var lines = File.ReadAllLines(fname);
            return lines.Select(CreateCommand).Where(c => c != null).ToArray();
        }
    }
}
