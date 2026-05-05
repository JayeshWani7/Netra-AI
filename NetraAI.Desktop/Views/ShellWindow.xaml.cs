using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Main application shell that hosts all screens in a single window.
    /// </summary>
    public partial class ShellWindow : Window
    {

        private Services.HotkeyManager? _hotkeyManager;
        private OverlayWindow? _overlayWindow;
        private readonly ILogger _logger;

        public ShellWindow()
        {
            InitializeComponent();
            _logger = Logger.GetInstance();
            Loaded += ShellWindow_Loaded;
            Closed += ShellWindow_Closed;
        }

        private void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize HotkeyManager
                _hotkeyManager = new Services.HotkeyManager(this);

                var failedHotkeys = new System.Collections.Generic.List<string>();

                RegisterHotkeySafe("Ctrl+Alt+A", Key.A, ToggleOverlay, failedHotkeys);
                RegisterHotkeySafe("Ctrl+Alt+G", Key.G, CaptureScreen, failedHotkeys);
                RegisterHotkeySafe("Ctrl+Alt+R", Key.R, SelectRegion, failedHotkeys);

                if (failedHotkeys.Count > 0)
                {
                    _logger.Warning($"Some hotkeys could not be registered: {string.Join(", ", failedHotkeys)}");
                    MessageBox.Show(
                        $"These hotkeys are already in use and were skipped: {string.Join(", ", failedHotkeys)}\n\n" +
                        "The app will still run, but those shortcuts will not work until they are changed.",
                        "Hotkey Conflict",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error($"Failed to initialize hotkey manager: {ex.Message}", ex);
                MessageBox.Show(
                    "Global hotkeys could not be initialized. The app will still run, but shortcuts may not work.",
                    "Hotkey Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void RegisterHotkeySafe(string display, Key key, System.Action callback, System.Collections.Generic.List<string> failures)
        {
            try
            {
                _hotkeyManager?.RegisterHotkey(
                    Services.HotkeyManager.Modifiers.Control | Services.HotkeyManager.Modifiers.Alt,
                    (uint)KeyInterop.VirtualKeyFromKey(key),
                    callback);
            }
            catch (System.Exception ex)
            {
                _logger.Warning($"Hotkey registration failed for {display}: {ex.Message}");
                failures.Add(display);
            }
        }

        private void ShellWindow_Closed(object? sender, System.EventArgs e)
        {
            _hotkeyManager?.Dispose();
        }

        private void ToggleOverlay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_overlayWindow == null)
                {
                    _overlayWindow = new OverlayWindow();
                    _overlayWindow.Closed += (s, e) => _overlayWindow = null;
                }
                if (_overlayWindow.IsVisible)
                {
                    _overlayWindow.Hide();
                }
                else
                {
                    _overlayWindow.Show();
                    _overlayWindow.Activate();
                }
            });
        }

        private void CaptureScreen()
        {
            // TODO: Implement screen capture logic
            MessageBox.Show("Screen capture triggered!", "Hotkey", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectRegion()
        {
            // TODO: Implement region selection logic
            MessageBox.Show("Region selection triggered!", "Hotkey", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public ContentControl GetContentHost()
        {
            return ShellContentHost;
        }
    }
}
