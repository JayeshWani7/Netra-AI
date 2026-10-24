using System;
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
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets whether the service provider container is initialized
        /// </summary>
        public static bool IsInitialized => _provider != null;

        /// <summary>
        /// Initialize dependency injection container with optional custom service configuration
        /// </summary>
        public static void Initialize(Action<IServiceCollection>? configureServices = null)
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
                    services.AddSingleton<GeminiService>();
                    services.AddSingleton<ScreenCaptureService>();

                    configureServices?.Invoke(services);

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
        /// Attempts to resolve a service instance safely
        /// </summary>
        public static bool TryGetService<T>(out T? service) where T : class
        {
            service = GetService<T>();
            return service != null;
        }

        /// <summary>
        /// Resets the dependency injection container and disposes existing provider if applicable
        /// </summary>
        public static void Reset()
        {
            lock (_lockObject)
            {
                if (_provider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _provider = null;
            }
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
