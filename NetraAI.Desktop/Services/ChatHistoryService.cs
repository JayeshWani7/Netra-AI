using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Handles per-user chat history persistence.
    /// </summary>
    public class ChatHistoryService
    {
        private readonly ILogger _logger;
        private readonly string _chatHistoryFolder;

        public event EventHandler<ChatHistoryUpdatedEventArgs>? ChatHistoryUpdated;

        public ChatHistoryService(ILogger logger)
        {
            _logger = logger;
            _chatHistoryFolder = Path.Combine(Constants.AppDataPath, Constants.ChatHistoryFolderName);
        }

        public Task<List<ChatMessage>> GetMessagesAsync(string userId)
        {
            try
            {
                var path = GetUserHistoryPath(userId);
                var messages = JsonHelper.DeserializeFromFile<List<ChatMessage>>(path) ?? new List<ChatMessage>();
                var ordered = messages.OrderBy(message => message.Timestamp).ToList();
                return Task.FromResult(ordered);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load chat history: {ex.Message}", ex);
                return Task.FromResult(new List<ChatMessage>());
            }
        }

        public Task<bool> AppendMessagesAsync(string userId, IEnumerable<ChatMessage> messages)
        {
            try
            {
                var path = GetUserHistoryPath(userId);
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);

                var existing = JsonHelper.DeserializeFromFile<List<ChatMessage>>(path) ?? new List<ChatMessage>();
                existing.AddRange(messages);

                var trimmed = existing
                    .OrderBy(message => message.Timestamp)
                    .TakeLast(Constants.MaxChatHistoryMessages)
                    .ToList();

                var result = JsonHelper.SerializeToFile(trimmed, path);
                if (result)
                {
                    OnChatHistoryUpdated(userId, trimmed.Count);
                }
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save chat history: {ex.Message}", ex);
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteMessagesAsync(string userId, IEnumerable<Guid> messageIds)
        {
            try
            {
                var ids = new HashSet<Guid>(messageIds);
                if (ids.Count == 0)
                {
                    return Task.FromResult(true);
                }

                var path = GetUserHistoryPath(userId);
                var existing = JsonHelper.DeserializeFromFile<List<ChatMessage>>(path) ?? new List<ChatMessage>();
                var remaining = existing.Where(message => !ids.Contains(message.Id)).ToList();

                var result = JsonHelper.SerializeToFile(remaining, path);
                if (result)
                {
                    OnChatHistoryUpdated(userId, remaining.Count);
                }
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to delete chat history: {ex.Message}", ex);
                return Task.FromResult(false);
            }
        }

        private void OnChatHistoryUpdated(string userId, int messageCount)
        {
            ChatHistoryUpdated?.Invoke(this, new ChatHistoryUpdatedEventArgs(userId, messageCount));
        }

        private string GetUserHistoryPath(string userId)
        {
            var safeUserId = SanitizeFileName(string.IsNullOrWhiteSpace(userId) ? "anonymous" : userId);
            return Path.Combine(_chatHistoryFolder, $"{safeUserId}.json");
        }

        private static string SanitizeFileName(string input)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(input.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "anonymous" : sanitized;
        }
    }

    public class ChatHistoryUpdatedEventArgs : EventArgs
    {
        public ChatHistoryUpdatedEventArgs(string userId, int messageCount)
        {
            UserId = userId;
            MessageCount = messageCount;
        }

        public string UserId { get; }
        public int MessageCount { get; }
    }
}
