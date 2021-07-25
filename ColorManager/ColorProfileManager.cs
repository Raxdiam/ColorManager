using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ColorManager.Json;

namespace ColorManager
{
    public class ColorProfileManager
    {
        public event EventHandler ProfilesLoaded;

        private readonly string _manifestFile;

        private Dictionary<string, string> _profilesManifest;

        public ColorProfileManager()
        {
            Profiles = new Dictionary<string, Func<Task<ColorProfile>>>();
            _manifestFile = Path.Combine(Program.AppData, "profiles.json");
            _profilesManifest = new Dictionary<string, string>();
        }

        public Dictionary<string, Func<Task<ColorProfile>>> Profiles { get; }

        public async Task CreateProfileAsync(string name, IEnumerable<ColorItem> items = null)
        {
            if (!Directory.Exists(Program.Config.ProfilesDirectory))
                Directory.CreateDirectory(Program.Config.ProfilesDirectory);

            var fileName = name.ToSafeFileName() + ".json";
            var profile = new ColorProfile(fileName);
            if (items != null) profile.Colors.AddRange(items);

            _profilesManifest.Add(name, fileName);

            await SaveManifestAsync();

            await profile.SaveAsync();
            await LoadAsync();
        }

        public async Task<bool> EditProfileAsync(string oldName, string newName)
        {
            if (!_profilesManifest.ContainsKey(oldName)) return false;

            try {
                var oldFileName = _profilesManifest[oldName];
                var newFileName = newName.ToSafeFileName() + ".json";

                _profilesManifest.Remove(oldName);
                _profilesManifest.Add(newName, newFileName);

                Profiles.Remove(oldName);
                Profiles.Add(newName, async () => await ColorProfile.LoadAsync(newFileName));

                File.Move(
                    Path.Combine(Program.Config.ProfilesDirectory, oldFileName),
                    Path.Combine(Program.Config.ProfilesDirectory, newFileName));

                await SaveManifestAsync();
            }
            catch {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteProfileAsync(string name)
        {
            if (!_profilesManifest.ContainsKey(name)) return false;

            try {
                File.Delete(Path.Combine(Program.Config.ProfilesDirectory, _profilesManifest[name]));

                _profilesManifest.Remove(name);
                Profiles.Remove(name);

                await SaveManifestAsync();
            }
            catch {
                return false;
            }

            return true;
        }

        public void Load() => LoadAsync().GetAwaiter().GetResult();

        public async Task LoadAsync()
        {
            Profiles.Clear();

            if (!File.Exists(_manifestFile)) {
                _profilesManifest = new Dictionary<string, string>();
                await SaveManifestAsync();
            }
            else {
                var json = File.ReadAllText(_manifestFile);
                var manifest = await JsonHelper.DeserializeAsync<Dictionary<string, string>>(json);
                _profilesManifest = manifest ?? new Dictionary<string, string>();
            }

            foreach (var kv in _profilesManifest) {
                Profiles.Add(kv.Key, async () => await ColorProfile.LoadAsync(kv.Value));

                var proFile = Path.Combine(Program.Config.ProfilesDirectory, kv.Value);
                if (!File.Exists(proFile)) {
                    var profile = new ColorProfile(kv.Value);
                    await profile.SaveAsync();
                }
            }

            ProfilesLoaded?.Invoke(this, EventArgs.Empty);
        }

        private async Task SaveManifestAsync()
        {
            var manifestJson = await JsonHelper.SerializeAsync(_profilesManifest);
            File.WriteAllText(_manifestFile, manifestJson);
        }
    }
}
