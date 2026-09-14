using System;
using Xunit;
using NetraAI.Desktop.Models;

namespace NetraAI.Tests
{
    public class AppConfigTests
    {
        [Fact]
        public void IsAuthenticated_ValidTokenAndUserId_ReturnsTrue()
        {
            var config = new AppConfig
            {
                UserId = "user-123",
                AuthToken = "token-xyz"
            };

            Assert.True(config.IsAuthenticated);
        }

        [Theory]
        [InlineData("", "token-xyz")]
        [InlineData("user-123", "")]
        [InlineData(null, "token-xyz")]
        [InlineData("user-123", null)]
        [InlineData("  ", "  ")]
        public void IsAuthenticated_MissingUserIdOrToken_ReturnsFalse(string? userId, string? token)
        {
            var config = new AppConfig
            {
                UserId = userId!,
                AuthToken = token
            };

            Assert.False(config.IsAuthenticated);
        }

        [Fact]
        public void ClearAuthentication_ResetsAuthFields()
        {
            var config = new AppConfig
            {
                UserId = "user-123",
                AuthToken = "token-xyz",
                RefreshToken = "refresh-xyz",
                Email = "test@example.com",
                DisplayName = "Tester",
                RememberMe = true
            };

            config.ClearAuthentication();

            Assert.Equal(string.Empty, config.UserId);
            Assert.Null(config.AuthToken);
            Assert.Null(config.RefreshToken);
            Assert.Null(config.Email);
            Assert.Null(config.DisplayName);
            Assert.False(config.RememberMe);
            Assert.False(config.IsAuthenticated);
        }

        [Theory]
        [InlineData("dark", true)]
        [InlineData("LIGHT", true)]
        [InlineData("system", false)]
        [InlineData("invalid_theme", false)]
        public void IsValidTheme_EvaluatesThemeName(string theme, bool expectedIsValid)
        {
            var config = new AppConfig { Theme = theme };
            Assert.Equal(expectedIsValid, config.IsValidTheme());
        }

        [Fact]
        public void ResetToDefaults_RestoresDefaultSettings()
        {
            var config = new AppConfig
            {
                Theme = "custom",
                Hotkey = "Ctrl+Shift+Z",
                AutoStart = true,
                Permissions = new AppConfig.PermissionSettings { ScreenAccess = true }
            };

            config.ResetToDefaults();

            Assert.Equal("dark", config.Theme);
            Assert.Equal("Ctrl+Alt+A", config.Hotkey);
            Assert.False(config.AutoStart);
            Assert.NotNull(config.Permissions);
            Assert.False(config.Permissions!.HasAnyPermission);
        }

        [Fact]
        public void PermissionSettings_HasAnyPermission_EvaluatesCorrectly()
        {
            var permissions = new AppConfig.PermissionSettings();
            Assert.False(permissions.HasAnyPermission);

            permissions.ScreenAccess = true;
            Assert.True(permissions.HasAnyPermission);
        }
    }
}
