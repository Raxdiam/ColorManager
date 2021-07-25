using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ColorManager.Json;

namespace ColorManager
{
    public class ColorProfile : IDisposable
    {
        private string _fileName;

        public ColorProfile() { }

        public ColorProfile(string fileName)
        {
            _fileName = fileName;
        }
        
        public List<ColorItem> Colors { get; set; } = new();
        
        public async Task SaveAsync()
        {
            var json = await JsonHelper.SerializeAsync(this);
            File.WriteAllText(Path.Combine(Program.Config.ProfilesDirectory, _fileName), json);
        }

        public void Save() => SaveAsync().GetAwaiter().GetResult();

        public static async Task<ColorProfile> LoadAsync(string fileName)
        {
            var file = Path.Combine(Program.Config.ProfilesDirectory, fileName);
            if (!File.Exists(file))
                throw new FileNotFoundException("Color profile does not exist.");
            var json = File.ReadAllText(file);
            var profile = await JsonHelper.DeserializeAsync<ColorProfile>(json);
            if (profile == null)
                throw new Exception("Could not load color profile.");
            profile._fileName = fileName;
            return profile;
        }

        public static ColorProfile Load(string fileName) => LoadAsync(fileName).GetAwaiter().GetResult();

        public void Dispose()
        {
            foreach (var item in Colors) {
                item.Dispose();
            }
        }
    }
}
