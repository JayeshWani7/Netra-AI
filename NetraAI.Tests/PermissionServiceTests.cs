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
            _permissionsPath = Path.Combine(Constants.AppDataPath, Constants.PermissionsFileName);
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

        [Fact]
        public async Task HasPermission_ReturnsCorrectStatus()
        {
            Assert.False(_service.HasScreenAccess());
            Assert.False(_service.HasMicrophoneAccess());
            Assert.False(_service.HasBackgroundRunning());
            Assert.False(_service.HasClipboardAccess());

            var permissions = new Permission
            {
                UserId = "test-user",
                ScreenAccess = true,
                MicrophoneAccess = false,
                BackgroundRunning = true,
                ClipboardAccess = true,
                IsExplicitlyRequested = true
            };

            await _service.SavePermissionsAsync(permissions);

            Assert.True(_service.HasScreenAccess());
            Assert.False(_service.HasMicrophoneAccess());
            Assert.True(_service.HasBackgroundRunning());
            Assert.True(_service.HasClipboardAccess());
        }

        [Fact]
        public void Constructor_CustomFilePath_InitializesProperties()
        {
            var mockLogger = new Mock<ILogger>();
            var customPath = Path.Combine(Path.GetTempPath(), $"custom_perm_{Guid.NewGuid()}.json");
            var service = new PermissionService(mockLogger.Object, customPath);

            Assert.Equal(customPath, service.PermissionsFilePath);
            Assert.False(service.PermissionsFileExists);
            Assert.Null(service.CurrentPermissions);
        }

        [Fact]
        public async Task HasAnyAndAllPermissions_EvaluatesAggregates()
        {
            var mockLogger = new Mock<ILogger>();
            var customPath = Path.Combine(Path.GetTempPath(), $"custom_perm_{Guid.NewGuid()}.json");
            var service = new PermissionService(mockLogger.Object, customPath);

            try
            {
                Assert.False(service.HasAnyPermission());
                Assert.False(service.HasAllPermissions());

                var permissions = new Permission
                {
                    UserId = "user-1",
                    ScreenAccess = true,
                    IsExplicitlyRequested = true
                };

                await service.SavePermissionsAsync(permissions);

                Assert.True(service.HasAnyPermission());
                Assert.False(service.HasAllPermissions());

                permissions.GrantAll();
                await service.SavePermissionsAsync(permissions);

                Assert.True(service.HasAnyPermission());
                Assert.True(service.HasAllPermissions());
            }
            finally
            {
                if (File.Exists(customPath)) File.Delete(customPath);
            }
        }

        [Fact]
        public async Task ResetPermissionsAsync_RemovesFileAndResetsState()
        {
            var mockLogger = new Mock<ILogger>();
            var customPath = Path.Combine(Path.GetTempPath(), $"custom_perm_{Guid.NewGuid()}.json");
            var service = new PermissionService(mockLogger.Object, customPath);

            try
            {
                var permissions = new Permission { UserId = "user-1", ScreenAccess = true, IsExplicitlyRequested = true };
                await service.SavePermissionsAsync(permissions);

                Assert.True(service.PermissionsFileExists);
                Assert.NotNull(service.CurrentPermissions);

                var resetResult = await service.ResetPermissionsAsync();

                Assert.True(resetResult);
                Assert.False(service.PermissionsFileExists);
                Assert.Null(service.CurrentPermissions);
            }
            finally
            {
                if (File.Exists(customPath)) File.Delete(customPath);
            }
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
