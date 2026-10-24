using System;
using Xunit;
using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Services;

namespace NetraAI.Tests
{
    public class ServiceProviderTests
    {
        [Fact]
        public void Initialize_RegistersAllServices_AndCanResolveSingletonInstances()
        {
            ServiceProvider.Initialize();

            Assert.True(ServiceProvider.IsInitialized);

            var logger = ServiceProvider.GetRequiredService<ILogger>();
            Assert.NotNull(logger);

            var authService = ServiceProvider.GetRequiredService<IAuthService>();
            Assert.NotNull(authService);

            var settingsService = ServiceProvider.GetRequiredService<SettingsService>();
            Assert.NotNull(settingsService);

            var permissionService = ServiceProvider.GetRequiredService<PermissionService>();
            Assert.NotNull(permissionService);

            var navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            Assert.NotNull(navigationService);

            var chatHistoryService = ServiceProvider.GetRequiredService<ChatHistoryService>();
            Assert.NotNull(chatHistoryService);

            var geminiService = ServiceProvider.GetRequiredService<GeminiService>();
            Assert.NotNull(geminiService);

            var screenCaptureService = ServiceProvider.GetRequiredService<ScreenCaptureService>();
            Assert.NotNull(screenCaptureService);
        }

        [Fact]
        public void GetService_ReturnsResolvedServiceInstance()
        {
            ServiceProvider.Initialize();

            var settingsService = ServiceProvider.GetService<SettingsService>();
            Assert.NotNull(settingsService);
        }

        [Fact]
        public void TryGetService_RegisteredService_ReturnsTrueAndInstance()
        {
            ServiceProvider.Initialize();

            var success = ServiceProvider.TryGetService<SettingsService>(out var service);
            Assert.True(success);
            Assert.NotNull(service);
        }

        [Fact]
        public void TryGetService_UnregisteredService_ReturnsFalseAndNull()
        {
            ServiceProvider.Initialize();

            var success = ServiceProvider.TryGetService<ServiceProviderTests>(out var service);
            Assert.False(success);
            Assert.Null(service);
        }

        [Fact]
        public void Reset_ClearsContainer_AndIsInitializedBecomesFalse()
        {
            ServiceProvider.Initialize();
            Assert.True(ServiceProvider.IsInitialized);

            ServiceProvider.Reset();
            Assert.False(ServiceProvider.IsInitialized);

            // Re-initialize for subsequent tests
            ServiceProvider.Initialize();
            Assert.True(ServiceProvider.IsInitialized);
        }
    }
}
