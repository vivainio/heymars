using System.Collections.Generic;

namespace GuiLaunch;

public class CommandEntry
{
    public string id { get; set; }
    public string c { get; set; }
    public string cwd { get; set; }

    public string title { get; set; }
    public bool? shell { get; set; }
    public string fgcolor { get; set; }
    public string bgcolor { get; set; }
    public List<string> runtags { get; set; }
    public List<string> tags { get; set; } 

    public override string ToString()
    {
        return c + (cwd == null ? "" : " || Cwd: " + cwd);  
    }

}