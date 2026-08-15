using System;
using Xunit;
using NetraAI.Desktop.Models;

namespace NetraAI.Tests
{
    public class UserModelTests
    {
        [Fact]
        public void GetFormattedDisplayName_DisplayNameProvided_ReturnsDisplayName()
        {
            var user = new User { DisplayName = "  John Doe  ", Email = "john@example.com" };
            Assert.Equal("John Doe", user.GetFormattedDisplayName());
        }

        [Fact]
        public void GetFormattedDisplayName_NoDisplayName_ReturnsEmail()
        {
            var user = new User { DisplayName = "", Email = "john@example.com" };
            Assert.Equal("john@example.com", user.GetFormattedDisplayName());
        }

        [Fact]
        public void GetFormattedDisplayName_NoDisplayNameOrEmail_ReturnsFallback()
        {
            var user = new User { DisplayName = "", Email = "" };
            Assert.Equal("CustomFallback", user.GetFormattedDisplayName("CustomFallback"));
        }

        [Fact]
        public void IsValid_ValidUserIdAndEmail_ReturnsTrue()
        {
            var user = new User { UserId = "user-123", Email = "john@example.com" };
            Assert.True(user.IsValid());
        }

        [Fact]
        public void IsValid_MissingUserIdOrEmail_ReturnsFalse()
        {
            var user1 = new User { UserId = "", Email = "john@example.com" };
            Assert.False(user1.IsValid());

            var user2 = new User { UserId = "user-123", Email = "" };
            Assert.False(user2.IsValid());
        }

        [Fact]
        public void ChatMessage_RoleProperties_IdentifiesUserAndAssistant()
        {
            var userMsg = new ChatMessage { Role = "USER" };
            Assert.True(userMsg.IsUser);
            Assert.False(userMsg.IsAssistant);

            var assistantMsg = new ChatMessage { Role = "assistant" };
            Assert.False(assistantMsg.IsUser);
            Assert.True(assistantMsg.IsAssistant);
        }

        [Fact]
        public void ChatSession_GetTotalTokensUsed_CalculatesSum()
        {
            var session = new ChatSession();
            session.AddMessage(new ChatMessage { Role = "user", TokensUsed = 10 });
            session.AddMessage(new ChatMessage { Role = "assistant", TokensUsed = 25 });
            session.AddMessage(new ChatMessage { Role = "user", TokensUsed = null });

            Assert.Equal(35, session.GetTotalTokensUsed());
        }
    }
}
