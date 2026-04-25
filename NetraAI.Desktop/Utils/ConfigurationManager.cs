using Microsoft.Extensions.Configuration;
using NetraAI.Desktop.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace NetraAI.Desktop.Utils
{
    /// <summary>
    /// Configuration manager for loading app settings
    /// </summary>
    public static class ConfigurationManager
    {
        private static IConfiguration? _configuration;
        private static FirebaseConfig? _firebaseConfig;
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "appsettings.json"
        );

        /// <summary>
        /// Initialize configuration from appsettings.json
        /// </summary>
        public static void Initialize()
        {
            try
            {
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _configuration = configBuilder.Build();
                
                // Load Firebase config
                _firebaseConfig = _configuration.GetSection("Firebase").Get<FirebaseConfig>();
                
                if (_firebaseConfig == null || !_firebaseConfig.IsValid())
                {
                    Logger.GetInstance().Warning("Firebase configuration is invalid or missing");
                }
                else
                {
                    Logger.GetInstance().Info($"Firebase configured for project: {_firebaseConfig.ProjectId}");
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to initialize configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get Firebase configuration
        /// </summary>
        public static FirebaseConfig? GetFirebaseConfig()
        {
            if (_firebaseConfig == null)
            {
                Initialize();
            }
            return _firebaseConfig;
        }

        /// <summary>
        /// Get configuration section
        /// </summary>
        public static IConfigurationSection? GetSection(string key)
        {
            if (_configuration == null)
            {
                Initialize();
            }
            return _configuration?.GetSection(key);
        }

        /// <summary>
        /// Get configuration value
        /// </summary>
        public static string? GetValue(string key)
        {
            if (_configuration == null)
            {
                Initialize();
            }
            return _configuration?[key];
        }
    }
}
