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
    }
}
