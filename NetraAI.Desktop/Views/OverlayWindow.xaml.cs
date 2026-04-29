using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private const double ExpandedWidth = 420;
        private const double ExpandedHeight = 520;
        private const double HiddenWidth = 8;
        private const double HiddenHeight = 8;

        public bool IsHidden { get; private set; } = true;
        private readonly ScreenCaptureService _screenCaptureService;
        private readonly GeminiService _geminiService;
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly ChatHistoryService _chatHistoryService;
        private byte[]? _attachedScreenshotPng;

        public OverlayWindow()
        {
            InitializeComponent();
            _screenCaptureService = new ScreenCaptureService();
            _geminiService = new GeminiService();
            _logger = Logger.GetInstance();
            _authService = ServiceProvider.GetRequiredService<IAuthService>();
            _chatHistoryService = ServiceProvider.GetRequiredService<ChatHistoryService>();
        }

        public void ToggleHidden()
        {
            if (IsHidden)
            {
                ShowExpanded();
            }
            else
            {
                ShowHidden();
            }
        }

        private void ShowExpanded()
        {
            IsHidden = false;
            Width = ExpandedWidth;
            Height = ExpandedHeight;
            Opacity = 1;
            IsHitTestVisible = true;
            ShowInTaskbar = false;
            Topmost = true;
            ToggleButton.Content = "Hide";
            if (!IsVisible)
            {
                Show();
            }
            Activate();
        }

        private void ShowHidden()
        {
            IsHidden = true;
            Width = HiddenWidth;
            Height = HiddenHeight;
            Opacity = 0;
            IsHitTestVisible = false;
            ShowInTaskbar = false;
            Topmost = true;
            ToggleButton.Content = "Show";
            if (!IsVisible)
            {
                Show();
            }
        }

        private void DragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsHidden && e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleHidden();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHidden();
        }

        private void UseScreenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UseScreenButton.IsEnabled = false;
                StatusText.Text = "Capturing screen...";

                _attachedScreenshotPng = _screenCaptureService.CapturePrimaryScreenPng();
                ResponseText.Text = "Screen attached. Add your message to send it with this image.";
                StatusText.Text = "Attachment ready.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Use Screen failed: {ex.Message}", ex);
                StatusText.Text = $"Failed: {ex.Message}";
            }
            finally
            {
                UseScreenButton.IsEnabled = true;
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var prompt = PromptTextBox.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(prompt) && (_attachedScreenshotPng == null || _attachedScreenshotPng.Length == 0))
                {
                    StatusText.Text = "Add a message or attach a screen first.";
                    return;
                }

                SendButton.IsEnabled = false;
                UseScreenButton.IsEnabled = false;
                StatusText.Text = "Sending to Gemini...";

                var user = _authService.GetCurrentUser();
                var userId = user?.UserId ?? "anonymous";
                var promptForHistory = string.IsNullOrWhiteSpace(prompt)
                    ? "Describe what is on my screen."
                    : prompt;

                var userMessage = new ChatMessage
                {
                    UserId = userId,
                    Role = "user",
                    Content = promptForHistory,
                    Timestamp = DateTime.UtcNow
                };

                await _chatHistoryService.AppendMessagesAsync(userId, new[] { userMessage });

                var response = await _geminiService.GenerateAsync(prompt, _attachedScreenshotPng, CancellationToken.None);
                ResponseText.Text = response.Trim();
                StatusText.Text = "Done.";
                _attachedScreenshotPng = null;
                PromptTextBox.Text = string.Empty;

                var assistantMessage = new ChatMessage
                {
                    UserId = userId,
                    Role = "assistant",
                    Content = ResponseText.Text,
                    Timestamp = DateTime.UtcNow,
                    Model = ConfigurationManager.GetValue("Gemini:Model")
                };

                await _chatHistoryService.AppendMessagesAsync(userId, new[] { assistantMessage });
            }
            catch (Exception ex)
            {
                _logger.Error($"Send failed: {ex.Message}", ex);
                StatusText.Text = $"Failed: {ex.Message}";

                var user = _authService.GetCurrentUser();
                var userId = user?.UserId ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var errorMessage = new ChatMessage
                    {
                        UserId = userId,
                        Role = "assistant",
                        Content = $"Error: {ex.Message}",
                        Timestamp = DateTime.UtcNow,
                        Model = ConfigurationManager.GetValue("Gemini:Model")
                    };

                    await _chatHistoryService.AppendMessagesAsync(userId, new[] { errorMessage });
                }
            }
            finally
            {
                SendButton.IsEnabled = true;
                UseScreenButton.IsEnabled = true;
            }
        }
    }
}
