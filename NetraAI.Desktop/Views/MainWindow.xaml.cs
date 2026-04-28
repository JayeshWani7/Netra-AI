using System.Windows;
using System.Windows.Controls;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;

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
        private OverlayWindow? _overlayWindow;

        public MainWindow()
        {
            InitializeComponent();
            
            _authService = ServiceProvider.GetRequiredService<IAuthService>();
            _navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            _logger = Logger.GetInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("MainWindow loaded");
                LoadUserInfo();
                LoadChatHistory();
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

        private void LoadChatHistory()
        {
            try
            {
                _logger.Info("Loading chat history...");
                
                // TODO: Load actual chat history from database
                // For now, just show placeholder
                ChatHistoryList.Items.Clear();
                ChatHistoryList.Items.Add(new System.Windows.Controls.ListBoxItem 
                { 
                    Content = "No conversations yet", 
                    Foreground = (System.Windows.Media.Brush)FindResource("BrushText"),
                    Opacity = 0.6,
                    IsEnabled = false
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load chat history: {ex.Message}", ex);
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
