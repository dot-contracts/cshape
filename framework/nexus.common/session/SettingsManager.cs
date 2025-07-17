using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace nexus.common
{
    public sealed class SettingsManager : ISettingsProvider
    {
        private static readonly string FilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nexus.settings.json");

        private readonly Dictionary<string, string> _settings = new();
        private IConfiguration? _defaults;

        public static SettingsManager Instance { get; } = new();

        private SettingsManager() { }

        public async Task InitializeAsync(IConfiguration? defaultConfig = null)
        {
            _defaults = defaultConfig;

            try
            {
                if (File.Exists(FilePath))
                {
                    var json = await File.ReadAllTextAsync(FilePath);
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (loaded != null)
                    {
                        foreach (var kv in loaded)
                            _settings[kv.Key] = kv.Value;
                    }
                }
            }
            catch
            {
                // Ignore errors, fallback to defaults
            }
        }

        public async Task SaveAsync()
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }

        public void Set(string key, string value) => _settings[key] = value;

        public string? Get(string key)
        {
            return _settings.TryGetValue(key, out var value)
                ? value
                : _defaults?[key];
        }

        public string GetString(string key, string fallback = "") =>
            Get(key) ?? fallback;

        public int GetInt(string key, int fallback = 0) =>
            int.TryParse(Get(key), out var i) ? i : fallback;

        public bool GetBool(string key, bool fallback = false) =>
            bool.TryParse(Get(key), out var b) ? b : fallback;

        public double GetDouble(string key, double fallback = 0) =>
            double.TryParse(Get(key), out var d) ? d : fallback;

        public DateTime GetDateTime(string key, DateTime fallback) =>
            DateTime.TryParse(Get(key), out var dt) ? dt : fallback;
    }
}
