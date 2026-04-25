using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Net.Http;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Models;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IAuthService _authService;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;
        private bool _isSignUpMode = false;

        public LoginWindow()
        {
            InitializeComponent();
            
            _authService = ServiceProvider.GetRequiredService<IAuthService>();
            _navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            _logger = Logger.GetInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.Info("LoginWindow loaded");
            EmailTextBox.Focus();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            HandleAuthentication();
        }

        private async void HandleAuthentication()
        {
            try
            {
                // Validate inputs
                string email = EmailTextBox.Text?.Trim() ?? string.Empty;
                string password = PasswordBox.Password ?? string.Empty;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ShowError("Please enter email and password");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    ShowError("Please enter a valid email address");
                    return;
                }

                if (password.Length < 6)
                {
                    ShowError("Password must be at least 6 characters");
                    return;
                }

                // Show loading state
                SetLoading(true);
                ClearError();

                User? user = null;

                if (_isSignUpMode)
                {
                    // Sign up
                    string displayName = DisplayNameTextBox.Text?.Trim() ?? email.Split('@')[0];
                    _logger.Info($"Attempting signup for: {email}");
                    user = await _authService.SignupAsync(email, password, displayName);
                }
                else
                {
                    // Login
                    _logger.Info($"Attempting login for: {email}");
                    user = await _authService.LoginAsync(email, password);
                }

                if (user != null)
                {
                    _logger.Info($"Authentication successful for user: {user.UserId}");
                    
                    // Navigate to permissions or main window
                    _navigationService.NavigateToPermissions(this);
                }
                else
                {
                    ShowError("Authentication failed. Please check your credentials.");
                    _logger.Warning("Authentication returned null user");
                }
            }
            catch (HttpRequestException ex)
            {
                ShowError("Network error. Please check your internet connection.");
                _logger.Error($"Network error: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
                _logger.Warning($"Authentication validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
                _logger.Error($"Authentication error: {ex.Message}", ex);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ToggleAuth_Click(object sender, RoutedEventArgs e)
        {
            _isSignUpMode = !_isSignUpMode;
            UpdateUIForAuthMode();
        }

        private void SignInTab_Click(object sender, RoutedEventArgs e)
        {
            if (_isSignUpMode)
            {
                _isSignUpMode = false;
                UpdateUIForAuthMode();
            }
        }

        private void SignUpTab_Click(object sender, RoutedEventArgs e)
        {
            if (!_isSignUpMode)
            {
                _isSignUpMode = true;
                UpdateUIForAuthMode();
            }
        }

        private void UpdateUIForAuthMode()
        {
            if (_isSignUpMode)
            {
                // Sign up mode
                AuthButton.Content = "Sign Up";
                DisplayNamePanel.Visibility = Visibility.Visible;
                RememberMeCheckbox.Visibility = Visibility.Collapsed;
                ForgotPasswordLink.Visibility = Visibility.Collapsed;
                
                SignUpTab.Background = (System.Windows.Media.Brush)FindResource("BrushPrimary");
                SignUpTab.Foreground = (System.Windows.Media.Brush)FindResource("BrushText");
                SignInTab.Background = (System.Windows.Media.Brush)FindResource("BrushSurface") ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));
            }
            else
            {
                // Sign in mode
                AuthButton.Content = "Sign In";
                DisplayNamePanel.Visibility = Visibility.Collapsed;
                RememberMeCheckbox.Visibility = Visibility.Visible;
                ForgotPasswordLink.Visibility = Visibility.Visible;
                
                SignInTab.Background = (System.Windows.Media.Brush)FindResource("BrushPrimary");
                SignInTab.Foreground = (System.Windows.Media.Brush)FindResource("BrushText");
                SignUpTab.Background = (System.Windows.Media.Brush)FindResource("BrushSurface") ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));
            }

            ClearError();
            ClearInputs();
        }

        private void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            ShowError("Google Sign-In is coming soon!");
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ShowError("Password reset is coming soon! Contact support.");
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void ClearError()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
            ErrorMessage.Text = string.Empty;
        }

        private void SetLoading(bool isLoading)
        {
            LoadingPanel.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            AuthButton.IsEnabled = !isLoading;
            EmailTextBox.IsEnabled = !isLoading;
            PasswordBox.IsEnabled = !isLoading;
            DisplayNameTextBox.IsEnabled = !isLoading;
            SignInTab.IsEnabled = !isLoading;
            SignUpTab.IsEnabled = !isLoading;
        }

        private void ClearInputs()
        {
            EmailTextBox.Clear();
            PasswordBox.Clear();
            DisplayNameTextBox.Clear();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
