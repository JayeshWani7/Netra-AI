namespace NetraAI.Desktop.Models
{
    /// <summary>
    /// Represents an application user
    /// </summary>
    public class User
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string? AuthToken { get; set; }
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Gets user display name or falls back to email or custom fallback
        /// </summary>
        public string GetFormattedDisplayName(string fallback = "User")
        {
            if (!string.IsNullOrWhiteSpace(DisplayName)) return DisplayName.Trim();
            if (!string.IsNullOrWhiteSpace(Email)) return Email.Trim();
            return fallback;
        }

        /// <summary>
        /// Checks whether the user object has valid identity details
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Email);
        }
    }
}
