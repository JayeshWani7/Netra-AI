using Xunit;
using NetraAI.Desktop.Models;

namespace NetraAI.Tests
{
    public class FirebaseConfigTests
    {
        [Fact]
        public void IsValid_DefaultConfig_ReturnsFalse()
        {
            var config = new FirebaseConfig();
            Assert.False(config.IsValid());
        }

        [Fact]
        public void IsValid_AllRequiredFieldsProvided_ReturnsTrue()
        {
            var config = new FirebaseConfig
            {
                ApiKey = "test-api-key",
                AuthDomain = "test-domain.firebaseapp.com",
                ProjectId = "test-project",
                StorageBucket = "test-bucket.appspot.com",
                MessagingSenderId = "123456789",
                AppId = "1:123456789:web:abcdef"
            };

            Assert.True(config.IsValid());
        }

        [Fact]
        public void IsValid_WhitespaceField_ReturnsFalse()
        {
            var config = new FirebaseConfig
            {
                ApiKey = "   ",
                AuthDomain = "test-domain.firebaseapp.com",
                ProjectId = "test-project",
                StorageBucket = "test-bucket.appspot.com",
                MessagingSenderId = "123456789",
                AppId = "1:123456789:web:abcdef"
            };

            Assert.False(config.IsValid());
        }

        [Fact]
        public void IsGoogleAuthConfigured_ValidClientId_ReturnsTrue()
        {
            var config = new FirebaseConfig
            {
                GoogleClientId = "test-client-id.apps.googleusercontent.com"
            };

            Assert.True(config.IsGoogleAuthConfigured());
        }

        [Fact]
        public void IsGoogleOAuthFullyConfigured_AllFieldsSet_ReturnsTrue()
        {
            var config = new FirebaseConfig
            {
                GoogleClientId = "test-client-id.apps.googleusercontent.com",
                GoogleClientSecret = "test-client-secret",
                GoogleRedirectUri = "http://localhost:5000/callback"
            };

            Assert.True(config.IsGoogleOAuthFullyConfigured());
        }

        [Fact]
        public void IsGoogleOAuthFullyConfigured_MissingSecret_ReturnsFalse()
        {
            var config = new FirebaseConfig
            {
                GoogleClientId = "test-client-id.apps.googleusercontent.com",
                GoogleClientSecret = "",
                GoogleRedirectUri = "http://localhost:5000/callback"
            };

            Assert.False(config.IsGoogleOAuthFullyConfigured());
        }
    }
}
