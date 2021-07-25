using System.IO;
using Newtonsoft.Json;

namespace ColorManager
{
    public class Config
    {
        private string _file;

        public string ProfilesDirectory { get; set; } = Path.Combine(Program.AppData, "Profiles");

        public int LensZoomLevel { get; set; } = 14;

        public bool TopMostWindow { get; set; } = true;

        public bool CopyHexHashtag { get; set; } = false;

        public void Save() => File.WriteAllText(_file, JsonConvert.SerializeObject(this, Formatting.Indented));

        public static Config Load(string file)
        {
            Config config;
            
            if (!File.Exists(file)) {
                config = new Config {_file = file};
                config.Save();
            }
            else {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(file)) ?? new Config();
                config._file = file;
            }

            return config;
        }
    }
}
