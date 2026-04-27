using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Xunit;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Models;

namespace NetraAI.Tests
{
    public class PermissionServiceTests : IDisposable
    {
        private readonly PermissionService _service;
        private readonly string _permissionsPath;

        public PermissionServiceTests()
        {
            var mockLogger = new Mock<ILogger>();
            _service = new PermissionService(mockLogger.Object);
            _permissionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NetraAI", "permissions.json");
            if (File.Exists(_permissionsPath)) File.Delete(_permissionsPath);
        }

        [Fact]
        public async Task SaveAndLoadPermissions_PersistsToDisk()
        {
            var permissions = new Permission
            {
                PermissionId = Guid.NewGuid().ToString(),
                UserId = "test-user",
                ScreenAccess = true,
                MicrophoneAccess = false,
                BackgroundRunning = true,
                ClipboardAccess = false,
                GrantedAt = DateTime.UtcNow,
                IsExplicitlyRequested = true
            };

            var saved = await _service.SavePermissionsAsync(permissions);
            Assert.True(saved);

            var loaded = await _service.LoadPermissionsAsync();
            Assert.NotNull(loaded);
            Assert.Equal(permissions.ScreenAccess, loaded.ScreenAccess);
            Assert.Equal(permissions.BackgroundRunning, loaded.BackgroundRunning);
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_permissionsPath)) File.Delete(_permissionsPath);
            }
            catch { }
        }
    }
}
