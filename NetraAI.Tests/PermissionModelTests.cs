using System;
using Xunit;
using NetraAI.Desktop.Models;

namespace NetraAI.Tests
{
    public class PermissionModelTests
    {
        [Fact]
        public void Permission_DefaultValues_AreDisabled()
        {
            var permission = new Permission();

            Assert.False(permission.HasScreenAccess());
            Assert.False(permission.HasMicrophoneAccess());
            Assert.False(permission.HasBackgroundRunning());
            Assert.False(permission.HasClipboardAccess());
            Assert.False(permission.HasAnyPermission);
            Assert.False(permission.HasAllPermissions);
            Assert.False(permission.IsValid);
        }

        [Fact]
        public void Permission_IsExplicitlyRequestedFalse_ReturnsFalseForAccess()
        {
            var permission = new Permission
            {
                UserId = "user-1",
                ScreenAccess = true,
                MicrophoneAccess = true,
                IsExplicitlyRequested = false
            };

            Assert.False(permission.HasScreenAccess());
            Assert.False(permission.HasMicrophoneAccess());
            Assert.False(permission.HasAnyPermission);
            Assert.True(permission.IsValid);
        }

        [Fact]
        public void GrantAll_SetsAllFlagsAndExplicitlyRequested()
        {
            var permission = new Permission { UserId = "user-123" };
            var beforeTime = DateTime.UtcNow;

            permission.GrantAll();

            Assert.True(permission.ScreenAccess);
            Assert.True(permission.MicrophoneAccess);
            Assert.True(permission.BackgroundRunning);
            Assert.True(permission.ClipboardAccess);
            Assert.True(permission.IsExplicitlyRequested);
            Assert.True(permission.HasAnyPermission);
            Assert.True(permission.HasAllPermissions);
            Assert.True(permission.GrantedAt >= beforeTime);
        }

        [Fact]
        public void RevokeAll_ClearsAllFlagsAndExplicitlyRequested()
        {
            var permission = new Permission { UserId = "user-123" };
            permission.GrantAll();

            var beforeTime = DateTime.UtcNow;
            permission.RevokeAll();

            Assert.False(permission.ScreenAccess);
            Assert.False(permission.MicrophoneAccess);
            Assert.False(permission.BackgroundRunning);
            Assert.False(permission.ClipboardAccess);
            Assert.False(permission.IsExplicitlyRequested);
            Assert.False(permission.HasAnyPermission);
            Assert.False(permission.HasAllPermissions);
            Assert.True(permission.GrantedAt >= beforeTime);
        }

        [Theory]
        [InlineData("user-123", true)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void IsValid_EvaluatesUserId(string? userId, bool expectedIsValid)
        {
            var permission = new Permission { UserId = userId! };
            Assert.Equal(expectedIsValid, permission.IsValid);
        }
    }
}
