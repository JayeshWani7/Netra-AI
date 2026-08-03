using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class SettingsServiceTests : IDisposable
    {
        private readonly SettingsService _service;
        private readonly string _settingsFilePath;

        public SettingsServiceTests()
        {
            _service = new SettingsService();
            _settingsFilePath = Path.Combine(Constants.ConfigPath, Constants.SettingsFileName);
            CleanupSettingsFile();
        }

        [Fact]
        public async Task SaveAndLoadSettings_PersistsSuccessfully()
        {
            var config = new AppConfig
            {
                UserId = "test-user-id",
                RememberMe = true,
                Theme = "light",
                Hotkey = "Ctrl+Shift+B",
                LastUpdated = DateTime.UtcNow
            };

            var saved = await _service.SaveAsync(config);
            Assert.True(saved);

            var loaded = await _service.LoadAsync();
            Assert.NotNull(loaded);
            Assert.Equal(config.UserId, loaded.UserId);
            Assert.Equal(config.RememberMe, loaded.RememberMe);
            Assert.Equal(config.Theme, loaded.Theme);
            Assert.Equal(config.Hotkey, loaded.Hotkey);
        }

        [Fact]
        public void GetConfig_ReturnsInstance()
        {
            var config = _service.GetConfig();
            Assert.NotNull(config);
        }

        [Fact]
        public async Task ResetToDefaultsAsync_ResetsConfigToDefaults()
        {
            var customConfig = new AppConfig
            {
                UserId = "custom-user",
                Theme = "light",
                Hotkey = "Ctrl+Shift+X",
                RememberMe = true
            };

            await _service.SaveAsync(customConfig);

            var resetResult = await _service.ResetToDefaultsAsync();
            Assert.True(resetResult);

            var loaded = await _service.LoadAsync();
            Assert.NotNull(loaded);
            Assert.Equal("dark", loaded.Theme);
            Assert.Equal("Ctrl+Alt+A", loaded.Hotkey);
            Assert.False(loaded.RememberMe);
            Assert.Empty(loaded.UserId);
        }

        private void CleanupSettingsFile()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                }
            }
            catch { }
        }

        public void Dispose()
        {
            CleanupSettingsFile();
        }
    }
}
