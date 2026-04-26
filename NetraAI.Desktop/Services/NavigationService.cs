using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Views;
using System.Windows;
using System.Windows.Controls;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for navigation between windows
    /// </summary>
    public class NavigationService
    {
        private readonly ILogger _logger;
        private Window? _shellWindow;
        private ContentControl? _contentHost;

        public NavigationService(ILogger logger)
        {
            _logger = logger;
            _logger.Info("Navigation service initialized for single-window navigation");
        }

        public void InitializeShell(Window shellWindow, ContentControl contentHost)
        {
            _shellWindow = shellWindow;
            _contentHost = contentHost;
            _logger.Info("Navigation shell initialized");
        }

        /// <summary>
        /// Navigate to permissions view after successful login
        /// </summary>
        public void NavigateToPermissions(object? source = null)
        {
            NavigateToView(new PermissionsWindow(), "Permissions");
        }

        /// <summary>
        /// Navigate to main dashboard view
        /// </summary>
        public void NavigateToMain(object? source = null)
        {
            NavigateToView(new MainWindow(), "Main");
        }

        /// <summary>
        /// Navigate back to login
        /// </summary>
        public void NavigateToLogin(object? source = null)
        {
            NavigateToView(new LoginWindow(), "Login");
        }

        /// <summary>
        /// Navigate to a specific view by name
        /// </summary>
        public void NavigateTo(string viewName)
        {
            try
            {
                _logger.Info($"Navigating to: {viewName}");

                switch (viewName)
                {
                    case "Login":
                        NavigateToLogin();
                        break;
                    case "Permissions":
                        NavigateToPermissions();
                        break;
                    case "Main":
                        NavigateToMain();
                        break;
                    default:
                        _logger.Warning($"View '{viewName}' not registered");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Navigation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Close current shell window
        /// </summary>
        public void CloseWindow()
        {
            _logger.Info("Closing current window");
            _shellWindow?.Close();
            _shellWindow = null;
            _contentHost = null;
        }

        /// <summary>
        /// Get current active window
        /// </summary>
        public Window? GetCurrentWindow()
        {
            return _shellWindow;
        }

        private void NavigateToView(UserControl view, string viewName)
        {
            try
            {
                if (_contentHost == null)
                {
                    _logger.Warning("Navigation shell is not initialized");
                    return;
                }

                _logger.Info($"Navigating to {viewName} view in shell");
                _contentHost.Content = view;
            }
            catch (Exception ex)
            {
                _logger.Error($"Navigation to {viewName} failed: {ex.Message}", ex);
            }
        }
    }
}
