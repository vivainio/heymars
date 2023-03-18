using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GuiLaunch;

public class Matcher
{
    public List<string> patterns { get; set; }
    public string say { get; set; }
    public bool log { get; set; } = false;
    public static bool MatchAny(Matcher m, string line) =>
        m.patterns == null ? false : m.patterns.Where(pat => Regex.IsMatch(line, pat)).Any();
}


public class CommandEntry
{
    public int index { get; set; }
    public string id { get; set; }
    public string c { get; set; }
    public string cwd { get; set; }

    public string title { get; set; }
    public bool? shell { get; set; }
    public string fgcolor { get; set; }
    public string bgcolor { get; set; }
    public List<string> runtags { get; set; }
    public List<string> tags { get; set; }
    public List<Matcher> matchers { get; set; }

    public override string ToString()
    {
        return c + (cwd == null ? "" : " || Cwd: " + cwd);  
    }

}