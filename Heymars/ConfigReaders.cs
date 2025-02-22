using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Spectre.Console;
using Tomlyn.Model;

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

        public static ConfigFile ReadTomlFile(string fname)
        {
            var cont = File.ReadAllText(fname);

            var cfg = Tomlyn.Toml.ToModel(cont);

            string gets(TomlTable table, string key)
            {
                return table.TryGetValue(key, out var v) ? v as string: null;
            }

            var root = gets(cfg, "root");
            cfg.Remove("root");
            var commands = cfg
                .Select(c => (key: c.Key, table: c.Value as TomlTable))
                .Select(pair => new CommandEntry
                {
                    title = pair.key,
                    id = gets(pair.table, "id"),
                    c = gets(pair.table, "c"),
                    cwd = gets(pair.table, "cwd"),
                    fgcolor = gets(pair.table, "fgcolor"),
                    bgcolor = gets(pair.table, "bgcolor"),
                    shell = pair.table.TryGetValue("shell", out var v) ? (bool)v : null,
                    runtags = pair.table.TryGetValue("runtags", out var vv) ? (vv as TomlArray).Select(v => v as string).ToList() : null,
                    tags = pair.table.TryGetValue("tags", out var vvv) ? (vvv as TomlArray).Select(v => v as string).ToList() : null,

                });
            return new ConfigFile
            {
                root = root,
                commands = commands.ToArray(),
            };
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
