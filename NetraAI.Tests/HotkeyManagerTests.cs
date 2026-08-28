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
    }
}
