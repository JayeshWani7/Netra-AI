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

        public PermissionService(ILogger logger, string? permissionsFilePath = null)
        {
            _logger = logger;
            _permissionsFilePath = !string.IsNullOrWhiteSpace(permissionsFilePath)
                ? permissionsFilePath
                : Path.Combine(Constants.AppDataPath, Constants.PermissionsFileName);
        }

        /// <summary>
        /// Gets the currently loaded permissions in memory
        /// </summary>
        public Permission? CurrentPermissions => _permissions;

        /// <summary>
        /// Gets the target path of the permissions file
        /// </summary>
        public string PermissionsFilePath => _permissionsFilePath;

        /// <summary>
        /// Checks whether the permissions file exists on disk
        /// </summary>
        public bool PermissionsFileExists => File.Exists(_permissionsFilePath);

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
        /// Check if user has clipboard access permission
        /// </summary>
        public bool HasClipboardAccess() => _permissions?.HasClipboardAccess() ?? false;

        /// <summary>
        /// Check if user has any active permission granted
        /// </summary>
        public bool HasAnyPermission() => _permissions?.HasAnyPermission ?? false;

        /// <summary>
        /// Check if user has all active permissions granted
        /// </summary>
        public bool HasAllPermissions() => _permissions?.HasAllPermissions ?? false;

        /// <summary>
        /// Resets and clears persisted permissions
        /// </summary>
        public Task<bool> ResetPermissionsAsync()
        {
            try
            {
                if (File.Exists(_permissionsFilePath))
                {
                    File.Delete(_permissionsFilePath);
                }
                _permissions = null;
                _logger.Info("Permissions reset successfully");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to reset permissions: {ex.Message}", ex);
                return Task.FromResult(false);
            }
        }

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
