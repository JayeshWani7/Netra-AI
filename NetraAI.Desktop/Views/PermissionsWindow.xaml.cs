using System.Windows;
using System.Windows.Controls;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Models;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for PermissionsWindow.xaml
    /// </summary>
    public partial class PermissionsWindow : UserControl
    {
        private readonly PermissionService _permissionService;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;

        public PermissionsWindow()
        {
            InitializeComponent();
            
            _permissionService = ServiceProvider.GetRequiredService<PermissionService>();
            _navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            _logger = Logger.GetInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("PermissionsWindow loaded");
                LoadSavedPermissions();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading permissions: {ex.Message}", ex);
            }
        }

        private void LoadSavedPermissions()
        {
            try
            {
                _logger.Info("Loading saved permissions...");
                var loaded = _permissionService.LoadPermissionsAsync().GetAwaiter().GetResult();
                if (loaded != null)
                {
                    ScreenCaptureCheckbox.IsChecked = loaded.ScreenAccess;
                    MicrophoneCheckbox.IsChecked = loaded.MicrophoneAccess;
                    BackgroundCheckbox.IsChecked = loaded.BackgroundRunning;
                    ClipboardCheckbox.IsChecked = loaded.ClipboardAccess;
                }
                // For now, set defaults:
                // - Screen Capture: required, checked by default
                // - Microphone: optional, unchecked
                // - Background: recommended, checked by default
                // - Clipboard: optional, unchecked
                
                _logger.Info("Permissions loaded");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load permissions: {ex.Message}", ex);
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("Save and continue clicked");

                SavePermissions();
                ProceedToMainWindow();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving permissions: {ex.Message}", ex);
                MessageBox.Show("Error saving permissions", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DenyAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("Deny All clicked");
                
                // Allow only screen capture (it's required)
                ScreenCaptureCheckbox.IsChecked = true;
                MicrophoneCheckbox.IsChecked = false;
                BackgroundCheckbox.IsChecked = false;
                ClipboardCheckbox.IsChecked = false;
                
                SavePermissions();
                ProceedToMainWindow();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error denying permissions: {ex.Message}", ex);
                MessageBox.Show("Error updating permissions", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SavePermissions()
        {
            try
            {
                _logger.Info("Saving permissions...");
                
                var permissions = new Permission
                {
                    PermissionId = Guid.NewGuid().ToString(),
                    UserId = ServiceProvider.GetService<IAuthService>()?.GetCurrentUser()?.UserId ?? string.Empty,
                    ScreenAccess = ScreenCaptureCheckbox.IsChecked ?? false,
                    MicrophoneAccess = MicrophoneCheckbox.IsChecked ?? false,
                    BackgroundRunning = BackgroundCheckbox.IsChecked ?? false,
                    ClipboardAccess = ClipboardCheckbox.IsChecked ?? false,
                    GrantedAt = DateTime.UtcNow,
                    IsExplicitlyRequested = true
                };
                
                _permissionService.SavePermissionsAsync(permissions).GetAwaiter().GetResult();
                _logger.Info($"Permissions saved: Screen={permissions.ScreenAccess}, Mic={permissions.MicrophoneAccess}, Background={permissions.BackgroundRunning}, Clipboard={permissions.ClipboardAccess}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save permissions: {ex.Message}", ex);
            }
        }

        private void ProceedToMainWindow()
        {
            try
            {
                _logger.Info("Proceeding to MainWindow");
                _navigationService.NavigateToMain(this);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to navigate to MainWindow: {ex.Message}", ex);
                MessageBox.Show("Error proceeding to main window", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
