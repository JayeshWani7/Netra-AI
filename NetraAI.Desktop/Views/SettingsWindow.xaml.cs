using System.Windows;
using System.Windows.Controls;
using NetraAI.Desktop.Models;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : UserControl
    {
        private readonly SettingsService _settingsService;
        private readonly PermissionService _permissionService;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;
        private AppConfig _config = new();

        public SettingsWindow()
        {
            InitializeComponent();

            _settingsService = ServiceProvider.GetRequiredService<SettingsService>();
            _permissionService = ServiceProvider.GetRequiredService<PermissionService>();
            _navigationService = ServiceProvider.GetRequiredService<NavigationService>();
            _logger = Logger.GetInstance();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.Info("SettingsWindow loaded");
                LoadSettings();
                await LoadPermissionsAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading settings: {ex.Message}", ex);
                MessageBox.Show("Failed to load settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            _config = _settingsService.GetConfig();

            HotkeyTextBox.Text = _config.Hotkey;
            AutoStartCheckbox.IsChecked = _config.AutoStart;

            if (string.Equals(_config.Theme, Constants.ThemeLight, StringComparison.OrdinalIgnoreCase))
            {
                ThemeComboBox.SelectedIndex = 1;
            }
            else
            {
                ThemeComboBox.SelectedIndex = 0;
            }
        }

        private async Task LoadPermissionsAsync()
        {
            var permissions = await _permissionService.LoadPermissionsAsync();
            if (permissions == null)
            {
                ScreenAccessCheckbox.IsChecked = true;
                MicrophoneAccessCheckbox.IsChecked = false;
                BackgroundAccessCheckbox.IsChecked = true;
                ClipboardAccessCheckbox.IsChecked = false;
                return;
            }

            ScreenAccessCheckbox.IsChecked = permissions.ScreenAccess;
            MicrophoneAccessCheckbox.IsChecked = permissions.MicrophoneAccess;
            BackgroundAccessCheckbox.IsChecked = permissions.BackgroundRunning;
            ClipboardAccessCheckbox.IsChecked = permissions.ClipboardAccess;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _config.Hotkey = HotkeyTextBox.Text?.Trim() ?? Constants.DefaultHotkey;
                _config.AutoStart = AutoStartCheckbox.IsChecked == true;
                _config.Theme = ThemeComboBox.SelectedIndex == 1 ? Constants.ThemeLight : Constants.ThemeDark;
                _config.LastUpdated = DateTime.UtcNow;

                await _settingsService.SaveAsync(_config);

                var screenAccess = ScreenAccessCheckbox.IsChecked == true;
                if (!screenAccess)
                {
                    ScreenAccessCheckbox.IsChecked = true;
                    screenAccess = true;
                }

                var permissions = new Permission
                {
                    PermissionId = Guid.NewGuid().ToString(),
                    UserId = string.Empty,
                    ScreenAccess = screenAccess,
                    MicrophoneAccess = MicrophoneAccessCheckbox.IsChecked == true,
                    BackgroundRunning = BackgroundAccessCheckbox.IsChecked == true,
                    ClipboardAccess = ClipboardAccessCheckbox.IsChecked == true,
                    GrantedAt = DateTime.UtcNow,
                    IsExplicitlyRequested = true
                };

                await _permissionService.SavePermissionsAsync(permissions);
                MessageBox.Show("Settings saved.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save settings: {ex.Message}", ex);
                MessageBox.Show("Failed to save settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateToMain(this);
        }
    }
}
