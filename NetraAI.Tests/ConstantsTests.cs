using Xunit;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class ConstantsTests
    {
        [Fact]
        public void Constants_Paths_AreNotNullOrEmpty()
        {
            Assert.False(string.IsNullOrWhiteSpace(Constants.AppDataPath));
            Assert.False(string.IsNullOrWhiteSpace(Constants.LogsPath));
            Assert.False(string.IsNullOrWhiteSpace(Constants.CachePath));
            Assert.False(string.IsNullOrWhiteSpace(Constants.ConfigPath));
        }

        [Fact]
        public void IsValidTheme_ValidThemes_ReturnsTrue()
        {
            Assert.True(Constants.IsValidTheme("dark"));
            Assert.True(Constants.IsValidTheme("light"));
            Assert.True(Constants.IsValidTheme("DARK"));
            Assert.True(Constants.IsValidTheme(" Light "));
        }

        [Fact]
        public void IsValidTheme_InvalidThemes_ReturnsFalse()
        {
            Assert.False(Constants.IsValidTheme(null));
            Assert.False(Constants.IsValidTheme(""));
            Assert.False(Constants.IsValidTheme("   "));
            Assert.False(Constants.IsValidTheme("blue"));
            Assert.False(Constants.IsValidTheme("custom"));
        }
    }
}
