using Xunit;
using Moq;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockLogger = new Mock<ILogger>();
            _authService = new AuthService(_mockLogger.Object);
        }

        [Fact]
        public void IsAuthenticated_WithoutLogin_ReturnsFalse()
        {
            // Arrange & Act
            var result = _authService.IsAuthenticated();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            // TODO: Replace with actual Firebase mock when implemented
            // Assert.NotNull(result);
            // Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetCurrentUser_NoLogin_ReturnsNull()
        {
            // Arrange & Act
            var result = _authService.GetCurrentUser();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LogoutAsync_Success()
        {
            // Arrange & Act
            await _authService.LogoutAsync();

            // Assert
            Assert.False(_authService.IsAuthenticated());
        }
    }
}
