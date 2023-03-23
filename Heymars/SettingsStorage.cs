using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GuiLaunch
{
    public class SettingsStorage
    {
        public string Location => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "heymars", "settings.json"); 
        public void LoadAndModify(Action<StoredSettings> modify)
        {
            StoredSettings storedSettings;
            if (!File.Exists(Location))
            {
                storedSettings = new StoredSettings
                {
                    history = new List<string>()
                };
                Directory.CreateDirectory(Path.GetDirectoryName(Location));
            } else
            {
                var cont = File.ReadAllBytes(Location);

                storedSettings = JsonSerializer.Deserialize<StoredSettings>(cont);
                storedSettings.history ??= new List<string>();

            }
            modify(storedSettings);
            var newCont = JsonSerializer.SerializeToUtf8Bytes(storedSettings);
            File.WriteAllBytes(Location, newCont);
        }
    }
}
