using System.IO;
using System.Text.Json;

namespace QuasarPad.Services
{
    public class AppSettings
    {
        public bool IsDarkMode { get; set; } = false;
        public bool IsPureMode { get; set; } = true;
        public bool WordWrap { get; set; } = true;
        public bool ShowLineNumbers { get; set; } = true;
        public bool ShowStatusBar { get; set; } = true;
        public string FontFamily { get; set; } = "Consolas";
        public double FontSize { get; set; } = 14;
        public double WindowWidth { get; set; } = 900;
        public double WindowHeight { get; set; } = 600;
        public double WindowLeft { get; set; } = -1;
        public double WindowTop { get; set; } = -1;
    }

    public class SettingsService
    {
        private readonly string _settingsPath;
        public AppSettings Settings { get; private set; }

        public SettingsService()
        {
            string appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "QuasarPad");

            Directory.CreateDirectory(appData);
            _settingsPath = Path.Combine(appData, "settings.json");
            Settings = Load();
        }

        public AppSettings Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(_settingsPath, json);
            }
            catch { }
        }
    }
}
