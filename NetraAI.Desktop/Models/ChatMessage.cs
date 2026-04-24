namespace NetraAI.Desktop.Models
{
    /// <summary>
    /// Represents a message in a chat session
    /// </summary>
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "user" or "assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid? ScreenshotId { get; set; }
        public double? Confidence { get; set; }
        public string? Model { get; set; } // AI model used (e.g., "gemini-pro")
        public int? TokensUsed { get; set; }
    }

    /// <summary>
    /// Represents a chat session
    /// </summary>
    public class ChatSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
        public List<ChatMessage> Messages { get; set; } = new();
        public string? Title { get; set; }
        public bool IsArchived { get; set; } = false;

        /// <summary>
        /// Add a message to this session
        /// </summary>
        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
            LastMessageAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Get conversation context (last N messages)
        /// </summary>
        public List<ChatMessage> GetContext(int maxMessages = 10)
        {
            return Messages.TakeLast(maxMessages).ToList();
        }
    }
}
