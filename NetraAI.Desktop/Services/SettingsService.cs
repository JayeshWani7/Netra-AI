using System;
using System.IO;
using System.Threading.Tasks;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for loading and saving application settings (AppConfig)
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private AppConfig? _config;

        public SettingsService()
        {
            _settingsFilePath = Path.Combine(Constants.ConfigPath, Constants.SettingsFileName);
        }

        public AppConfig GetConfig()
        {
            if (_config == null)
            {
                _config = LoadAsync().GetAwaiter().GetResult() ?? new AppConfig();
            }
            return _config!;
        }

        public async Task<AppConfig?> LoadAsync()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    _config = new AppConfig();
                    return _config;
                }

                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var config = JsonHelper.Deserialize<AppConfig>(json);
                _config = config ?? new AppConfig();
                return _config;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to load settings: {ex.Message}", ex);
                return null;
            }
        }

        public async Task<bool> SaveAsync(AppConfig config)
        {
            try
            {
                var directory = Path.GetDirectoryName(_settingsFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonHelper.Serialize(config);
                await File.WriteAllTextAsync(_settingsFilePath, json);
                _config = config;
                return true;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to save settings: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Resets application settings to default values and persists them.
        /// </summary>
        public async Task<bool> ResetToDefaultsAsync()
        {
            var defaultConfig = new AppConfig();
            return await SaveAsync(defaultConfig);
        }
    }
}
