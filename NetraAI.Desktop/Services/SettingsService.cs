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
            : this(Path.Combine(Constants.ConfigPath, Constants.SettingsFileName))
        {
        }

        public SettingsService(string settingsFilePath)
        {
            _settingsFilePath = !string.IsNullOrWhiteSpace(settingsFilePath)
                ? settingsFilePath
                : Path.Combine(Constants.ConfigPath, Constants.SettingsFileName);
        }

        /// <summary>
        /// Gets the target path of the settings JSON file
        /// </summary>
        public string SettingsFilePath => _settingsFilePath;

        /// <summary>
        /// Checks whether the settings JSON file exists on disk
        /// </summary>
        public bool SettingsFileExists => File.Exists(_settingsFilePath);

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

        /// <summary>
        /// Updates the current configuration using an update delegate and persists the updated settings
        /// </summary>
        public async Task<bool> UpdateConfigAsync(Action<AppConfig> updateAction)
        {
            if (updateAction == null)
                return false;

            var currentConfig = GetConfig();
            updateAction(currentConfig);
            currentConfig.LastUpdated = DateTime.UtcNow;
            return await SaveAsync(currentConfig);
        }
    }
}
