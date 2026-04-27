using System;
using System.IO;
using System.Threading.Tasks;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for managing application permissions
    /// </summary>
    public class PermissionService
    {
        private Permission? _permissions;
        private readonly string _permissionsFilePath;
        private readonly ILogger _logger;

        public PermissionService(ILogger logger)
        {
            _logger = logger;
            _permissionsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NetraAI",
                "permissions.json"
            );
        }

        /// <summary>
        /// Load permissions from storage
        /// </summary>
        public async Task<Permission?> LoadPermissionsAsync()
        {
            try
            {
                if (File.Exists(_permissionsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_permissionsFilePath);
                    _permissions = JsonHelper.Deserialize<Permission>(json);
                    _logger.Info("Permissions loaded successfully");
                    return _permissions;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load permissions: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Save permissions to storage
        /// </summary>
        public async Task<bool> SavePermissionsAsync(Permission permissions)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_permissionsFilePath) ?? "");
                var json = JsonHelper.Serialize(permissions);
                await File.WriteAllTextAsync(_permissionsFilePath, json);
                _permissions = permissions;
                _logger.Info("Permissions saved successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save permissions: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Check if user has screen access permission
        /// </summary>
        public bool HasScreenAccess() => _permissions?.HasScreenAccess() ?? false;

        /// <summary>
        /// Check if user has microphone access permission
        /// </summary>
        public bool HasMicrophoneAccess() => _permissions?.HasMicrophoneAccess() ?? false;

        /// <summary>
        /// Check if user has background running permission
        /// </summary>
        public bool HasBackgroundRunning() => _permissions?.HasBackgroundRunning() ?? false;

        /// <summary>
        /// Request permission from user
        /// </summary>
        public async Task<bool> RequestPermissionAsync(string permissionType)
        {
            _logger.Info($"Permission request: {permissionType}");
            // TODO: Show permission UI
            await Task.Delay(100);
            return true;
        }
    }
}
