using System;
using System.Collections.Generic;
using Xunit;
using NetraAI.Desktop.Models;

namespace NetraAI.Tests
{
    public class ChatMessageTests
    {
        [Theory]
        [InlineData("user", true, false)]
        [InlineData("USER", true, false)]
        [InlineData("assistant", false, true)]
        [InlineData("ASSISTANT", false, true)]
        [InlineData("system", false, false)]
        public void ChatMessage_RoleChecks_ReturnExpectedValues(string role, bool expectedIsUser, bool expectedIsAssistant)
        {
            var message = new ChatMessage { Role = role };
            Assert.Equal(expectedIsUser, message.IsUser);
            Assert.Equal(expectedIsAssistant, message.IsAssistant);
        }

        [Fact]
        public void ChatMessage_HasScreenshot_EvaluatesCorrectly()
        {
            var msgWithoutScreenshot = new ChatMessage();
            Assert.False(msgWithoutScreenshot.HasScreenshot);

            var msgWithEmptyGuid = new ChatMessage { ScreenshotId = Guid.Empty };
            Assert.False(msgWithEmptyGuid.HasScreenshot);

            var msgWithValidGuid = new ChatMessage { ScreenshotId = Guid.NewGuid() };
            Assert.True(msgWithValidGuid.HasScreenshot);
        }

        [Theory]
        [InlineData("user", "Hello AI", true)]
        [InlineData("assistant", "Hello User", true)]
        [InlineData("", "Hello", false)]
        [InlineData("user", "", false)]
        [InlineData("   ", "   ", false)]
        public void ChatMessage_IsValid_EvaluatesRoleAndContent(string role, string content, bool expectedIsValid)
        {
            var message = new ChatMessage { Role = role, Content = content };
            Assert.Equal(expectedIsValid, message.IsValid);
        }

        [Fact]
        public void ChatSession_AddMessage_UpdatesLastMessageAtAndMessagesList()
        {
            var session = new ChatSession();
            var initialTime = session.LastMessageAt;
            var message = new ChatMessage { Role = "user", Content = "Test" };

            session.AddMessage(message);

            Assert.Single(session.Messages);
            Assert.Equal(message, session.Messages[0]);
            Assert.True(session.LastMessageAt >= initialTime);
        }

        [Fact]
        public void ChatSession_GetContext_ReturnsLastNMessages()
        {
            var session = new ChatSession();
            for (int i = 1; i <= 15; i++)
            {
                session.AddMessage(new ChatMessage { Role = "user", Content = $"Message {i}" });
            }

            var context = session.GetContext(5);

            Assert.Equal(5, context.Count);
            Assert.Equal("Message 11", context[0].Content);
            Assert.Equal("Message 15", context[4].Content);
        }

        [Fact]
        public void ChatSession_GetLatestMessage_ReturnsMostRecentMessageOrNull()
        {
            var emptySession = new ChatSession();
            Assert.Null(emptySession.GetLatestMessage());
            Assert.True(emptySession.IsEmpty);

            var session = new ChatSession();
            var msg1 = new ChatMessage { Role = "user", Content = "First" };
            var msg2 = new ChatMessage { Role = "assistant", Content = "Second" };
            session.AddMessage(msg1);
            session.AddMessage(msg2);

            Assert.False(session.IsEmpty);
            Assert.Equal(msg2, session.GetLatestMessage());
        }

        [Fact]
        public void ChatSession_GetTotalTokensUsed_HandlesNullsAndSumsValues()
        {
            var session = new ChatSession();
            session.AddMessage(new ChatMessage { Role = "user", TokensUsed = 50 });
            session.AddMessage(new ChatMessage { Role = "assistant", TokensUsed = 120 });
            session.AddMessage(new ChatMessage { Role = "user", TokensUsed = null });

            Assert.Equal(170, session.GetTotalTokensUsed());
        }
    }
}
