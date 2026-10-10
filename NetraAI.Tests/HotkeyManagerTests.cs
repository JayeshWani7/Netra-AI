using Xunit;
using NetraAI.Desktop.Services;

namespace NetraAI.Tests
{
    public class HotkeyManagerTests
    {
        [Fact]
        public void ParseHotkeyString_CtrlAltA_ParsesCorrectly()
        {
            var success = HotkeyManager.ParseHotkeyString("Ctrl+Alt+A", out var modifiers, out var key);

            Assert.True(success);
            Assert.Equal(HotkeyManager.Modifiers.Control | HotkeyManager.Modifiers.Alt, modifiers);
            Assert.Equal((uint)'A', key);
        }

        [Fact]
        public void ParseHotkeyString_ShiftWinG_ParsesCorrectly()
        {
            var success = HotkeyManager.ParseHotkeyString("Shift+Win+G", out var modifiers, out var key);

            Assert.True(success);
            Assert.Equal(HotkeyManager.Modifiers.Shift | HotkeyManager.Modifiers.Win, modifiers);
            Assert.Equal((uint)'G', key);
        }

        [Fact]
        public void ParseHotkeyString_NullOrEmpty_ReturnsFalse()
        {
            Assert.False(HotkeyManager.ParseHotkeyString(null, out _, out _));
            Assert.False(HotkeyManager.ParseHotkeyString("", out _, out _));
            Assert.False(HotkeyManager.ParseHotkeyString("   ", out _, out _));
        }

        [Fact]
        public void ParseHotkeyString_NoKeyLetter_ReturnsFalse()
        {
            Assert.False(HotkeyManager.ParseHotkeyString("Ctrl+Alt", out _, out _));
        }

        [Theory]
        [InlineData("Ctrl+Alt+A", true)]
        [InlineData("Shift+G", true)]
        [InlineData("Win+Alt+R", true)]
        [InlineData("Ctrl+Alt", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValidHotkeyString_EvaluatesHotkeyFormat(string? hotkeyString, bool expectedResult)
        {
            var result = HotkeyManager.IsValidHotkeyString(hotkeyString);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void FormatHotkey_ValidModifiersAndKey_ReturnsFormattedString()
        {
            var formatted = HotkeyManager.FormatHotkey(
                HotkeyManager.Modifiers.Control | HotkeyManager.Modifiers.Alt,
                (uint)'A'
            );

            Assert.Equal("Ctrl+Alt+A", formatted);
        }

        [Fact]
        public void FormatHotkey_ZeroKey_ReturnsEmptyString()
        {
            var formatted = HotkeyManager.FormatHotkey(HotkeyManager.Modifiers.Control, 0);
            Assert.Equal(string.Empty, formatted);
        }
    }
}
