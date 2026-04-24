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
    }
}
