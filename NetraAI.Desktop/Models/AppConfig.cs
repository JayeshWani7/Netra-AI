using Newtonsoft.Json;

namespace NetraAI.Desktop.Models
{
    /// <summary>
    /// Application configuration and settings
    /// </summary>
    public class AppConfig
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("auth_token")]
        public string? AuthToken { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("display_name")]
        public string? DisplayName { get; set; }

        [JsonProperty("remember_me")]
        public bool RememberMe { get; set; } = false;

        [JsonProperty("theme")]
        public string Theme { get; set; } = "dark";

        [JsonProperty("hotkey")]
        public string Hotkey { get; set; } = "Ctrl+Alt+A";

        [JsonProperty("auto_start")]
        public bool AutoStart { get; set; } = false;

        [JsonProperty("permissions")]
        public PermissionSettings? Permissions { get; set; } = new();

        [JsonProperty("app_version")]
        public string AppVersion { get; set; } = "1.0.0";

        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Permission settings sub-object
        /// </summary>
        public class PermissionSettings
        {
            [JsonProperty("screen_access")]
            public bool ScreenAccess { get; set; } = false;

            [JsonProperty("mic_access")]
            public bool MicAccess { get; set; } = false;

            [JsonProperty("background_running")]
            public bool BackgroundRunning { get; set; } = false;

            [JsonProperty("clipboard_access")]
            public bool ClipboardAccess { get; set; } = false;
        }
    }
}
