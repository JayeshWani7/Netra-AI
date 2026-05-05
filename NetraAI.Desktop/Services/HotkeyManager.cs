using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NetraAI.Desktop.Services
{
    public enum HotkeyAction
    {
        ToggleOverlay,
        CaptureScreen,
        SelectRegion
    }

    public class HotkeyManager : IDisposable
    {
        private readonly Dictionary<int, Action> _hotkeyActions = new();
        private readonly HwndSource _source;
        private int _currentId = 0x0001;

        // Modifier keys
        [Flags]
        public enum Modifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public HotkeyManager(Window window)
        {
            var helper = new WindowInteropHelper(window);
            if (helper.Handle == IntPtr.Zero)
            {
                helper.EnsureHandle();
            }

            _source = HwndSource.FromHwnd(helper.Handle)
                ?? throw new InvalidOperationException("Failed to initialize hotkey hook window source.");
            _source.AddHook(HwndHook);
        }

        public int RegisterHotkey(Modifiers modifiers, uint key, Action callback)
        {
            int id = _currentId++;
            if (!RegisterHotKey(_source.Handle, id, (uint)modifiers, key))
            {
                var error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Could not register hotkey. Win32Error={error}.");
            }
            _hotkeyActions[id] = callback;
            return id;
        }

        public void UnregisterHotkey(int id)
        {
            UnregisterHotKey(_source.Handle, id);
            _hotkeyActions.Remove(id);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (_hotkeyActions.TryGetValue(id, out var action))
                {
                    action?.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            foreach (var id in _hotkeyActions.Keys)
                UnregisterHotKey(_source.Handle, id);
            _source.RemoveHook(HwndHook);
        }
    }
}
