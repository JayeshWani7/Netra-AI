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
        /// Gets avatar initials from display name or email
        /// </summary>
        public string GetInitials()
        {
            var name = !string.IsNullOrWhiteSpace(DisplayName) ? DisplayName.Trim() : Email.Trim();
            if (string.IsNullOrWhiteSpace(name)) return "U";

            var parts = name.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && parts[0].Length > 0 && parts[1].Length > 0)
            {
                return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[1][0])}";
            }
            return char.ToUpperInvariant(name[0]).ToString();
        }

        /// <summary>
        /// Checks whether the user object has valid identity details
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Email);
        }

        /// <summary>
        /// Checks whether active authentication tokens exist and identity is valid
        /// </summary>
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AuthToken) && IsValid();

        /// <summary>
        /// Updates the last login timestamp to current UTC time
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Clears stored authentication and refresh tokens
        /// </summary>
        public void ClearAuthentication()
        {
            AuthToken = null;
            RefreshToken = null;
        }
    }
}
