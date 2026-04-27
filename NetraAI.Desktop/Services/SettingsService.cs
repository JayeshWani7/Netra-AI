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
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath) ?? string.Empty);
                var config = JsonHelper.DeserializeFromFile<AppConfig>(_settingsFilePath);
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
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath) ?? string.Empty);
                var result = JsonHelper.SerializeToFile(config, _settingsFilePath);
                if (result)
                {
                    _config = config;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to save settings: {ex.Message}", ex);
                return false;
            }
        }
    }
}
