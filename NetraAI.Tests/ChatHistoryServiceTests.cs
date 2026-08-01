using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class ChatHistoryServiceTests : IDisposable
    {
        private readonly ChatHistoryService _service;
        private readonly string _testUserId;
        private readonly string _chatHistoryFolder;

        public ChatHistoryServiceTests()
        {
            var mockLogger = new Mock<ILogger>();
            _service = new ChatHistoryService(mockLogger.Object);
            _testUserId = $"test-user-{Guid.NewGuid():N}";
            _chatHistoryFolder = Path.Combine(Constants.AppDataPath, Constants.ChatHistoryFolderName);
        }

        [Fact]
        public async Task GetMessagesAsync_NoHistory_ReturnsEmptyList()
        {
            var messages = await _service.GetMessagesAsync(_testUserId);
            Assert.NotNull(messages);
            Assert.Empty(messages);
        }

        [Fact]
        public async Task AppendMessagesAsync_NullMessages_ReturnsFalse()
        {
            var result = await _service.AppendMessagesAsync(_testUserId, null!);
            Assert.False(result);
        }

        [Fact]
        public async Task AppendAndGetMessages_PersistsAndOrdersByTimestamp()
        {
            var msg1 = new ChatMessage
            {
                Id = Guid.NewGuid(),
                Role = "user",
                Content = "Hello",
                Timestamp = DateTime.UtcNow.AddMinutes(-5)
            };
            var msg2 = new ChatMessage
            {
                Id = Guid.NewGuid(),
                Role = "assistant",
                Content = "Hi there!",
                Timestamp = DateTime.UtcNow
            };

            var appended = await _service.AppendMessagesAsync(_testUserId, new[] { msg1, msg2 });
            Assert.True(appended);

            var retrieved = await _service.GetMessagesAsync(_testUserId);
            Assert.Equal(2, retrieved.Count);
            Assert.Equal("Hello", retrieved[0].Content);
            Assert.Equal("Hi there!", retrieved[1].Content);
        }

        [Fact]
        public async Task DeleteMessagesAsync_RemovesSpecifiedMessages()
        {
            var msg1 = new ChatMessage { Id = Guid.NewGuid(), Role = "user", Content = "Msg 1", Timestamp = DateTime.UtcNow };
            var msg2 = new ChatMessage { Id = Guid.NewGuid(), Role = "assistant", Content = "Msg 2", Timestamp = DateTime.UtcNow.AddSeconds(1) };
            
            await _service.AppendMessagesAsync(_testUserId, new[] { msg1, msg2 });
            
            var deleted = await _service.DeleteMessagesAsync(_testUserId, new[] { msg1.Id });
            Assert.True(deleted);

            var remaining = await _service.GetMessagesAsync(_testUserId);
            Assert.Single(remaining);
            Assert.Equal(msg2.Id, remaining[0].Id);
        }

        [Fact]
        public async Task ClearAllMessagesAsync_RemovesAllMessages()
        {
            var msg1 = new ChatMessage { Id = Guid.NewGuid(), Role = "user", Content = "Msg 1", Timestamp = DateTime.UtcNow };
            var msg2 = new ChatMessage { Id = Guid.NewGuid(), Role = "assistant", Content = "Msg 2", Timestamp = DateTime.UtcNow.AddSeconds(1) };

            await _service.AppendMessagesAsync(_testUserId, new[] { msg1, msg2 });

            var cleared = await _service.ClearAllMessagesAsync(_testUserId);
            Assert.True(cleared);

            var remaining = await _service.GetMessagesAsync(_testUserId);
            Assert.Empty(remaining);
        }

        public void Dispose()
        {
            try
            {
                var filePath = Path.Combine(_chatHistoryFolder, $"{_testUserId}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch { }
        }
    }
}
