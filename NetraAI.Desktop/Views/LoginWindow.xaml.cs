using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Models;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : UserControl
    {
        private readonly IAuthService _authService;
        private readonly NavigationService _navigationService;
        private readonly SettingsService _settingsService;
        private readonly ILogger _logger;
        private bool _isSignUpMode = false;

        public LoginWindow()
        {
            InitializeComponent();
            
            _authService = ServiceProvider.GetRequiredService<IAuthService>();
            _navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            _settingsService = ServiceProvider.GetRequiredService<SettingsService>();
            _logger = Logger.GetInstance();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.Info("LoginWindow loaded");
            var firebaseConfig = ConfigurationManager.GetFirebaseConfig();
            var googleConfigured = firebaseConfig?.IsGoogleAuthConfigured() == true;
            GoogleButton.IsEnabled = googleConfigured;
            GoogleConfigText.Visibility = googleConfigured ? Visibility.Collapsed : Visibility.Visible;

            var config = await _settingsService.LoadAsync();
            if (config != null)
            {
                RememberMeCheckbox.IsChecked = config.RememberMe;
                if (config.RememberMe && !string.IsNullOrWhiteSpace(config.AuthToken))
                {
                    var cachedUser = new User
                    {
                        UserId = config.UserId ?? string.Empty,
                        Email = config.Email ?? string.Empty,
                        DisplayName = config.DisplayName ?? string.Empty,
                        AuthToken = config.AuthToken,
                        RefreshToken = config.RefreshToken,
                        LastLoginAt = DateTime.UtcNow
                    };

                    _authService.RestoreSession(cachedUser);
                    var permissionService = ServiceProvider.GetRequiredService<PermissionService>();
                    var savedPermissions = await permissionService.LoadPermissionsAsync();

                    if (savedPermissions?.IsExplicitlyRequested == true)
                    {
                        _navigationService.NavigateToMain(this);
                        return;
                    }

                    _navigationService.NavigateToPermissions(this);
                    return;
                }
            }

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
                    await SaveSessionAsync(user, RememberMeCheckbox.IsChecked == true || _isSignUpMode);
                    var permissionService = ServiceProvider.GetRequiredService<PermissionService>();
                    var savedPermissions = await permissionService.LoadPermissionsAsync();

                    if (savedPermissions?.IsExplicitlyRequested == true)
                    {
                        _navigationService.NavigateToMain(this);
                    }
                    else
                    {
                        _navigationService.NavigateToPermissions(this);
                    }
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
                SignUpTab.Foreground = (System.Windows.Media.Brush)FindResource("BrushBackground");
                SignUpTab.BorderBrush = (System.Windows.Media.Brush)FindResource("BrushPrimary");
                SignInTab.Background = (System.Windows.Media.Brush)FindResource("BrushSurface") ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));
                SignInTab.Foreground = (System.Windows.Media.Brush)FindResource("BrushText");
                SignInTab.BorderBrush = (System.Windows.Media.Brush)FindResource("BrushDivider");
            }
            else
            {
                // Sign in mode
                AuthButton.Content = "Sign In";
                DisplayNamePanel.Visibility = Visibility.Collapsed;
                RememberMeCheckbox.Visibility = Visibility.Visible;
                ForgotPasswordLink.Visibility = Visibility.Visible;
                
                SignInTab.Background = (System.Windows.Media.Brush)FindResource("BrushPrimary");
                SignInTab.Foreground = (System.Windows.Media.Brush)FindResource("BrushBackground");
                SignInTab.BorderBrush = (System.Windows.Media.Brush)FindResource("BrushPrimary");
                SignUpTab.Background = (System.Windows.Media.Brush)FindResource("BrushSurface") ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));
                SignUpTab.Foreground = (System.Windows.Media.Brush)FindResource("BrushText");
                SignUpTab.BorderBrush = (System.Windows.Media.Brush)FindResource("BrushDivider");
            }

            ClearError();
            ClearInputs();
        }

        private async void GoogleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetLoading(true);
                ClearError();

                var user = await _authService.LoginWithGoogleAsync();
                if (user != null)
                {
                    _logger.Info($"Google authentication successful for user: {user.UserId}");
                    await SaveSessionAsync(user, RememberMeCheckbox.IsChecked == true || _isSignUpMode);
                    var permissionService = ServiceProvider.GetRequiredService<PermissionService>();
                    var savedPermissions = await permissionService.LoadPermissionsAsync();

                    if (savedPermissions?.IsExplicitlyRequested == true)
                    {
                        _navigationService.NavigateToMain(this);
                    }
                    else
                    {
                        _navigationService.NavigateToPermissions(this);
                    }
                    return;
                }

                ShowError("Google Sign-In failed. Please try again.");
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
                _logger.Warning($"Google authentication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError("Google Sign-In failed. Please try again.");
                _logger.Error($"Google authentication error: {ex.Message}", ex);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ShowError("Password reset is coming soon! Contact support.");
        }

        private async Task SaveSessionAsync(User user, bool rememberMe)
        {
            var config = _settingsService.GetConfig();
            config.UserId = user.UserId;
            config.Email = user.Email;
            config.DisplayName = user.DisplayName;
            config.AuthToken = rememberMe ? user.AuthToken : null;
            config.RefreshToken = rememberMe ? user.RefreshToken : null;
            config.RememberMe = rememberMe;
            config.LastUpdated = DateTime.UtcNow;

            await _settingsService.SaveAsync(config);
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
            GoogleButton.IsEnabled = !isLoading && (ConfigurationManager.GetFirebaseConfig()?.IsGoogleAuthConfigured() == true);
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
