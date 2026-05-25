using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
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
        private const double MinOverlayWidth = 320;
        private const double MinOverlayHeight = 360;

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
        }

        public async Task AutoCaptureAndAskAsync(string userQuestion)
        {
            try
            {
                // Show the overlay
                ShowExpanded();

                // Capture screen
                StatusText.Text = "Capturing screen...";
                _attachedScreenshotPng = _screenCaptureService.CapturePrimaryScreenPng();

                // Send to Gemini
                SendButton.IsEnabled = false;
                UseScreenButton.IsEnabled = false;
                StatusText.Text = "Processing with Gemini...";

                var user = _authService.GetCurrentUser();
                var userId = user?.UserId ?? "anonymous";

                var userMessage = new ChatMessage
                {
                    UserId = userId,
                    Role = "user",
                    Content = userQuestion,
                    Timestamp = DateTime.UtcNow
                };

                await _chatHistoryService.AppendMessagesAsync(userId, new[] { userMessage });

                var response = await _geminiService.GenerateAsync(userQuestion, _attachedScreenshotPng, CancellationToken.None);
                
                // Extract code from markdown block and clean comments
                var codeOnly = ExtractCodeFromMarkdown(response);
                var cleanedResponse = RemoveComments(codeOnly);
                
                ResponseText.Text = cleanedResponse.Trim();
                StatusText.Text = "Done.";
                _attachedScreenshotPng = null;
                PromptTextBox.Text = string.Empty;

                var assistantMessage = new ChatMessage
                {
                    UserId = userId,
                    Role = "assistant",
                    Content = cleanedResponse.Trim(),
                    Timestamp = DateTime.UtcNow
                };

                await _chatHistoryService.AppendMessagesAsync(userId, new[] { assistantMessage });
            }
            catch (Exception ex)
            {
                _logger.Error($"Auto capture and ask failed: {ex.Message}", ex);
                StatusText.Text = $"Error: {ex.Message}";
            }
            finally
            {
                SendButton.IsEnabled = true;
                UseScreenButton.IsEnabled = true;
            }
        }

        private string RemoveComments(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return code;

            var result = new System.Text.StringBuilder();
            var chars = code.ToCharArray();
            var i = 0;

            while (i < chars.Length)
            {
                // Check for single-line comment
                if (i < chars.Length - 1 && chars[i] == '/' && chars[i + 1] == '/')
                {
                    // Skip until end of line
                    while (i < chars.Length && chars[i] != '\n')
                        i++;
                    if (i < chars.Length)
                        result.Append('\n'); // Keep the newline
                    i++;
                    continue;
                }

                // Check for multi-line comment
                if (i < chars.Length - 1 && chars[i] == '/' && chars[i + 1] == '*')
                {
                    i += 2; // Skip /*
                    // Skip until */
                    while (i < chars.Length - 1)
                    {
                        if (chars[i] == '*' && chars[i + 1] == '/')
                        {
                            i += 2;
                            break;
                        }
                        if (chars[i] == '\n')
                            result.Append('\n'); // Preserve newlines inside comments
                        i++;
                    }
                    continue;
                }

                result.Append(chars[i]);
                i++;
            }

            // Clean up excessive blank lines
            var lines = result.ToString().Split('\n');
            var cleaned = new System.Collections.Generic.List<string>();
            var lastWasBlank = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    if (!lastWasBlank)
                        cleaned.Add("");
                    lastWasBlank = true;
                }
                else
                {
                    cleaned.Add(line); // Keep original indentation
                    lastWasBlank = false;
                }
            }

            return string.Join("\n", cleaned).Trim();
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

        private void ResizeGrip_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (IsHidden)
            {
                return;
            }

            var nextWidth = Math.Max(MinOverlayWidth, Width + e.HorizontalChange);
            var nextHeight = Math.Max(MinOverlayHeight, Height + e.VerticalChange);

            Width = nextWidth;
            Height = nextHeight;
        }

        private void ResizeGripTopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (IsHidden)
            {
                return;
            }

            var nextWidth = Math.Max(MinOverlayWidth, Width - e.HorizontalChange);
            var nextHeight = Math.Max(MinOverlayHeight, Height - e.VerticalChange);

            var widthChange = Width - nextWidth;
            var heightChange = Height - nextHeight;

            Width = nextWidth;
            Height = nextHeight;
            Left += widthChange;
            Top += heightChange;
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
                
                // Clean comments from the response
                var cleanedResponse = RemoveComments(response);
                
                ResponseText.Text = cleanedResponse.Trim();
                StatusText.Text = "Done.";
                _attachedScreenshotPng = null;
                PromptTextBox.Text = string.Empty;

                var assistantMessage = new ChatMessage
                {
                    UserId = userId,
                    Role = "assistant",
                    Content = cleanedResponse.Trim(),
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

        private string ExtractCodeFromMarkdown(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return response;

            var lines = response.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            var codeStartIndex = -1;
            var codeEndIndex = -1;

            // Find ```java or ``` (code block start)
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("```"))
                {
                    codeStartIndex = i;
                    break;
                }
            }

            // If no code block found, return original response
            if (codeStartIndex == -1)
                return response;

            // Find closing ``` (code block end)
            for (int i = codeStartIndex + 1; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("```"))
                {
                    codeEndIndex = i;
                    break;
                }
            }

            // If no closing found, return everything after opening
            if (codeEndIndex == -1)
                codeEndIndex = lines.Count;

            // Extract code between markers
            var codeLines = new System.Collections.Generic.List<string>();
            for (int i = codeStartIndex + 1; i < codeEndIndex; i++)
            {
                codeLines.Add(lines[i]);
            }

            return string.Join("\n", codeLines).Trim();
        }
    }
}

