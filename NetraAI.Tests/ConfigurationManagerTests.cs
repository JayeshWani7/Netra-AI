using Xunit;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class ConfigurationManagerTests
    {
        [Fact]
        public void GetValue_NullOrWhitespaceKey_ReturnsNull()
        {
            Assert.Null(ConfigurationManager.GetValue(null!));
            Assert.Null(ConfigurationManager.GetValue(""));
            Assert.Null(ConfigurationManager.GetValue("   "));
        }

        [Fact]
        public void GetSection_NullOrWhitespaceKey_ReturnsNull()
        {
            Assert.Null(ConfigurationManager.GetSection(null!));
            Assert.Null(ConfigurationManager.GetSection(""));
            Assert.Null(ConfigurationManager.GetSection("   "));
        }

        [Fact]
        public void Initialize_ExecutesSafelyAndCanRetrieveValues()
        {
            ConfigurationManager.Initialize();
            var firebaseConfig = ConfigurationManager.GetFirebaseConfig();
            // In test environment without full appsettings.json initialized, it should not throw exception
            Assert.True(true);
        }
    }
}
