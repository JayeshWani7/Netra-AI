namespace NetraAI.Desktop.Models
{
    /// <summary>
    /// Represents user permissions for app features
    /// </summary>
    public class Permission
    {
        public string PermissionId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public bool ScreenAccess { get; set; } = false;
        public bool MicrophoneAccess { get; set; } = false;
        public bool BackgroundRunning { get; set; } = false;
        public bool ClipboardAccess { get; set; } = false;
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public bool IsExplicitlyRequested { get; set; } = false;
        
        /// <summary>
        /// Check if user has permission for screen access
        /// </summary>
        public bool HasScreenAccess() => ScreenAccess && IsExplicitlyRequested;
        
        /// <summary>
        /// Check if user has permission for microphone access
        /// </summary>
        public bool HasMicrophoneAccess() => MicrophoneAccess && IsExplicitlyRequested;
        
        /// <summary>
        /// Check if user has permission for background execution
        /// </summary>
        public bool HasBackgroundRunning() => BackgroundRunning && IsExplicitlyRequested;

        /// <summary>
        /// Check if user has permission for clipboard access
        /// </summary>
        public bool HasClipboardAccess() => ClipboardAccess && IsExplicitlyRequested;

        /// <summary>
        /// Check if user has any active permission granted
        /// </summary>
        public bool HasAnyPermission => HasScreenAccess() || HasMicrophoneAccess() || HasBackgroundRunning() || HasClipboardAccess();

        /// <summary>
        /// Check if user has all active permissions granted
        /// </summary>
        public bool HasAllPermissions => HasScreenAccess() && HasMicrophoneAccess() && HasBackgroundRunning() && HasClipboardAccess();

        /// <summary>
        /// Check if the permission model has a valid associated user ID
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(UserId);

        /// <summary>
        /// Grant all permissions explicitly
        /// </summary>
        public void GrantAll()
        {
            ScreenAccess = true;
            MicrophoneAccess = true;
            BackgroundRunning = true;
            ClipboardAccess = true;
            IsExplicitlyRequested = true;
            GrantedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Revoke all permissions
        /// </summary>
        public void RevokeAll()
        {
            ScreenAccess = false;
            MicrophoneAccess = false;
            BackgroundRunning = false;
            ClipboardAccess = false;
            IsExplicitlyRequested = false;
            GrantedAt = DateTime.UtcNow;
        }
    }
}
