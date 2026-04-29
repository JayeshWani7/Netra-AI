using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        private readonly IAuthService _authService;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;
        private readonly ChatHistoryService _chatHistoryService;
        private OverlayWindow? _overlayWindow;

        public MainWindow()
        {
            InitializeComponent();
            
            _authService = ServiceProvider.GetRequiredService<IAuthService>();
            _navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            _logger = Logger.GetInstance();
            _chatHistoryService = ServiceProvider.GetRequiredService<ChatHistoryService>();
            _chatHistoryService.ChatHistoryUpdated += ChatHistoryService_ChatHistoryUpdated;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("MainWindow loaded");
                LoadUserInfo();
                await LoadChatHistoryAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading MainWindow: {ex.Message}", ex);
                MessageBox.Show("Error loading dashboard. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                // In a real app, you would get this from the session/storage
                // For now, we'll get it from properties or reload from auth
                _logger.Info("Loading user information...");

                var user = _authService.GetCurrentUser();
                var displayName = !string.IsNullOrWhiteSpace(user?.DisplayName)
                    ? user!.DisplayName
                    : (!string.IsNullOrWhiteSpace(user?.Email) ? user!.Email : "User");

                UserNameText.Text = displayName;
                WelcomeText.Text = "Ready to capture and analyze your screen!";
                
                _logger.Info("User information loaded");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load user info: {ex.Message}", ex);
            }
        }

        private async Task LoadChatHistoryAsync()
        {
            try
            {
                _logger.Info("Loading chat history...");

                var user = _authService.GetCurrentUser();
                var userId = user?.UserId ?? string.Empty;

                ChatHistoryList.Items.Clear();

                if (string.IsNullOrWhiteSpace(userId))
                {
                    AddChatHistoryPlaceholder("Login to view your chat history.");
                    QuestionCountText.Text = "0";
                    ShowWelcomePanel();
                    return;
                }

                var messages = await _chatHistoryService.GetMessagesAsync(userId);
                var questionCount = messages.Count(message => string.Equals(message.Role, "user", StringComparison.OrdinalIgnoreCase));
                QuestionCountText.Text = questionCount.ToString();

                if (messages.Count == 0)
                {
                    AddChatHistoryPlaceholder("No conversations yet");
                    ShowWelcomePanel();
                    return;
                }

                var pairs = BuildMessagePairs(messages);
                foreach (var pair in pairs.TakeLast(10))
                {
                    AddChatHistoryItem(pair.Question, pair.Answer);
                }

                ShowWelcomePanel();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load chat history: {ex.Message}", ex);
            }
        }

        private async void ChatHistoryService_ChatHistoryUpdated(object? sender, ChatHistoryUpdatedEventArgs e)
        {
            try
            {
                var user = _authService.GetCurrentUser();
                if (user == null || string.IsNullOrWhiteSpace(user.UserId))
                {
                    return;
                }

                if (!string.Equals(e.UserId, user.UserId, StringComparison.Ordinal))
                {
                    return;
                }

                if (Dispatcher.CheckAccess())
                {
                    await LoadChatHistoryAsync();
                }
                else
                {
                    await Dispatcher.InvokeAsync(async () => await LoadChatHistoryAsync());
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to refresh chat history: {ex.Message}", ex);
            }
        }

        private void AddChatHistoryPlaceholder(string text)
        {
            ChatHistoryList.Items.Add(new ListBoxItem
            {
                Content = text,
                Foreground = (Brush)FindResource("BrushText"),
                Opacity = 0.6,
                IsEnabled = false
            });
        }

        private static List<(ChatMessage? Question, ChatMessage? Answer)> BuildMessagePairs(List<ChatMessage> messages)
        {
            var ordered = messages.OrderBy(message => message.Timestamp).ToList();
            var pairs = new List<(ChatMessage? Question, ChatMessage? Answer)>();
            ChatMessage? currentQuestion = null;

            foreach (var message in ordered)
            {
                if (string.Equals(message.Role, "user", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion != null)
                    {
                        pairs.Add((currentQuestion, null));
                    }

                    currentQuestion = message;
                    continue;
                }

                if (string.Equals(message.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentQuestion == null)
                    {
                        pairs.Add((null, message));
                    }
                    else
                    {
                        pairs.Add((currentQuestion, message));
                        currentQuestion = null;
                    }
                }
            }

            if (currentQuestion != null)
            {
                pairs.Add((currentQuestion, null));
            }

            return pairs;
        }

        private void AddChatHistoryItem(ChatMessage? question, ChatMessage? answer)
        {
            var container = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            if (question != null)
            {
                var questionText = new TextBlock
                {
                    Text = TruncateText(question.Content, 160),
                    Foreground = (Brush)FindResource("BrushText"),
                    TextWrapping = TextWrapping.Wrap
                };
                Grid.SetColumn(questionText, 0);
                container.Children.Add(questionText);
            }

            if (question == null)
            {
                var placeholderText = new TextBlock
                {
                    Text = "Conversation entry",
                    Foreground = (Brush)FindResource("BrushMuted"),
                    TextWrapping = TextWrapping.Wrap
                };
                Grid.SetColumn(placeholderText, 0);
                container.Children.Add(placeholderText);
            }

            var deleteButton = new Button
            {
                Content = new TextBlock
                {
                    Text = "\uE74D",
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Width = 24,
                Height = 24,
                Padding = new Thickness(0),
                Margin = new Thickness(8, 0, 0, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = (Brush)FindResource("BrushMuted"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                ToolTip = "Delete"
            };
            deleteButton.Click += DeleteHistoryButton_Click;
            Grid.SetColumn(deleteButton, 1);
            container.Children.Add(deleteButton);

            var itemData = new ChatHistoryItemData(
                question?.Content ?? string.Empty,
                answer?.Content ?? string.Empty,
                question?.Id,
                answer?.Id
            );
            deleteButton.Tag = itemData;

            ChatHistoryList.Items.Add(new ListBoxItem
            {
                Content = container,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Tag = itemData
            });
        }

        private static string TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "(empty)";
            }

            var trimmed = text.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed.Substring(0, maxLength - 3) + "...";
        }

        private void ChatHistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatHistoryList.SelectedItem is not ListBoxItem item)
            {
                ShowWelcomePanel();
                return;
            }

            if (item.Tag is not ChatHistoryItemData data)
            {
                ShowWelcomePanel();
                return;
            }

            ShowChatDetail(data);
        }

        private void ShowChatDetail(ChatHistoryItemData data)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            ChatDetailPanel.Visibility = Visibility.Visible;
            ChatDetailQuestion.Text = string.IsNullOrWhiteSpace(data.Question) ? "(empty)" : data.Question.Trim();
            ChatDetailAnswer.Text = string.IsNullOrWhiteSpace(data.Answer) ? "No answer saved yet." : data.Answer.Trim();
        }

        private void ShowWelcomePanel()
        {
            ChatDetailPanel.Visibility = Visibility.Collapsed;
            WelcomePanel.Visibility = Visibility.Visible;
            ChatDetailQuestion.Text = string.Empty;
            ChatDetailAnswer.Text = string.Empty;
        }

        private async void DeleteHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (sender is not Button button || button.Tag is not ChatHistoryItemData data)
            {
                return;
            }

            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrWhiteSpace(user.UserId))
            {
                return;
            }

            var ids = data.GetMessageIds();
            var deleted = await _chatHistoryService.DeleteMessagesAsync(user.UserId, ids);
            if (deleted)
            {
                ChatHistoryList.SelectedItem = null;
                ShowWelcomePanel();
            }
        }

        private sealed class ChatHistoryItemData
        {
            public ChatHistoryItemData(string question, string answer, Guid? questionId, Guid? answerId)
            {
                Question = question;
                Answer = answer;
                QuestionId = questionId;
                AnswerId = answerId;
            }

            public string Question { get; }
            public string Answer { get; }
            public Guid? QuestionId { get; }
            public Guid? AnswerId { get; }

            public IEnumerable<Guid> GetMessageIds()
            {
                if (QuestionId.HasValue)
                {
                    yield return QuestionId.Value;
                }

                if (AnswerId.HasValue)
                {
                    yield return AnswerId.Value;
                }
            }
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("Start Netra button clicked");

                if (_overlayWindow == null)
                {
                    _overlayWindow = new OverlayWindow();
                    _overlayWindow.Closed += (_, _) => _overlayWindow = null;
                }

                _overlayWindow.ToggleHidden();
                StatusText.Text = _overlayWindow.IsHidden
                    ? "Overlay hidden (not visible on screen share)."
                    : "Overlay visible. Drag it anywhere on screen.";
            }
            catch (Exception ex)
            {
                _logger.Error($"Start Netra button error: {ex.Message}", ex);
                MessageBox.Show($"Unable to open the overlay.\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("Settings button clicked");
                _navigationService.NavigateToSettings(this);
            }
            catch (Exception ex)
            {
                _logger.Error($"Settings button error: {ex.Message}", ex);
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    _logger.Info("User initiated logout");
                    await _authService.LogoutAsync();

                    var settingsService = ServiceProvider.GetRequiredService<SettingsService>();
                    var config = settingsService.GetConfig();
                    config.AuthToken = null;
                    config.RefreshToken = null;
                    config.RememberMe = false;
                    config.LastUpdated = DateTime.UtcNow;
                    await settingsService.SaveAsync(config);
                    
                    _logger.Info("Logout successful, navigating to login");
                    _navigationService.NavigateToLogin(this);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Logout failed: {ex.Message}", ex);
                MessageBox.Show($"Logout failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _logger.Info("MainWindow closing");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error during window close: {ex.Message}", ex);
            }
        }
    }
}
