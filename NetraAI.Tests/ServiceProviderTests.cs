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
    }
}
