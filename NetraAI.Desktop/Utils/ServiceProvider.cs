using Microsoft.Extensions.DependencyInjection;
using NetraAI.Desktop.Services;

namespace NetraAI.Desktop.Utils
{
    /// <summary>
    /// Service provider for dependency injection
    /// </summary>
    public static class ServiceProvider
    {
        private static IServiceProvider? _provider;
        private static object _lockObject = new object();

        /// <summary>
        /// Initialize dependency injection container
        /// </summary>
        public static void Initialize()
        {
            lock (_lockObject)
            {
                if (_provider == null)
                {
                    // Initialize configuration first
                    ConfigurationManager.Initialize();

                    var services = new ServiceCollection();

                    // Register logging
                    services.AddSingleton<ILogger>(Logger.GetInstance());

                    // Register services
                    services.AddSingleton<IAuthService, AuthService>();
                    services.AddSingleton<SettingsService>();
                    services.AddSingleton<PermissionService>();
                    services.AddSingleton<NavigationService>();
                    services.AddSingleton<ChatHistoryService>();

                    _provider = services.BuildServiceProvider();

                    Logger.GetInstance().Info("Dependency injection container initialized");
                }
            }
        }

        /// <summary>
        /// Get service instance
        /// </summary>
        public static T? GetService<T>() where T : class
        {
            return _provider?.GetService<T>();
        }

        /// <summary>
        /// Get required service instance
        /// </summary>
        public static T GetRequiredService<T>() where T : class
        {
            if (_provider == null)
                throw new InvalidOperationException("Service provider not initialized");

            return _provider.GetRequiredService<T>();
        }
    }
}
