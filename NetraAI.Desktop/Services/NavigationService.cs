using NetraAI.Desktop.Utils;
using NetraAI.Desktop.Views;
using System.Windows;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for navigation between windows
    /// </summary>
    public class NavigationService
    {
        private readonly Dictionary<string, Type> _viewMap = new();
        private readonly ILogger _logger;
        private Window? _currentWindow;

        public NavigationService(ILogger logger)
        {
            _logger = logger;
            RegisterViews();
        }

        private void RegisterViews()
        {
            _viewMap["Login"] = typeof(LoginWindow);
            _viewMap["Permissions"] = typeof(PermissionsWindow);
            _viewMap["Main"] = typeof(MainWindow);
            _viewMap["Overlay"] = typeof(OverlayWindow);
            
            _logger.Info("Navigation service initialized with 4 registered views");
        }

        /// <summary>
        /// Navigate to permissions window after successful login
        /// </summary>
        public void NavigateToPermissions(Window fromWindow)
        {
            try
            {
                _logger.Info("Navigating to PermissionsWindow");
                
                var permissionsWindow = new PermissionsWindow();
                permissionsWindow.Show();
                Application.Current.MainWindow = permissionsWindow;
                
                // Close the current window
                fromWindow.Close();
                _currentWindow = permissionsWindow;
            }
            catch (Exception ex)
            {
                _logger.Error($"Navigation to Permissions failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Navigate to main dashboard window
        /// </summary>
        public void NavigateToMain(Window fromWindow)
        {
            try
            {
                _logger.Info("Navigating to MainWindow");
                
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Application.Current.MainWindow = mainWindow;
                
                // Close the current window
                fromWindow.Close();
                _currentWindow = mainWindow;
            }
            catch (Exception ex)
            {
                _logger.Error($"Navigation to Main failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Navigate back to login
        /// </summary>
        public void NavigateToLogin(Window fromWindow)
        {
            try
            {
                _logger.Info("Navigating back to LoginWindow");
                
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Application.Current.MainWindow = loginWindow;
                
                // Close the current window
                fromWindow.Close();
                _currentWindow = loginWindow;
            }
            catch (Exception ex)
            {
                _logger.Error($"Navigation to Login failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Navigate to a specific window by name
        /// </summary>
        public void NavigateTo(string windowName)
        {
            try
            {
                _logger.Info($"Navigating to: {windowName}");
                
                if (!_viewMap.ContainsKey(windowName))
                {
                    _logger.Warning($"Window '{windowName}' not registered");
                    return;
                }

                var windowType = _viewMap[windowName];
                var window = (Window?)Activator.CreateInstance(windowType);
                
                if (window != null)
                {
                    window.Show();
                    Application.Current.MainWindow = window;
                    _currentWindow?.Close();
                    _currentWindow = window;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Navigation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Close current window
        /// </summary>
        public void CloseWindow()
        {
            _logger.Info("Closing current window");
            _currentWindow?.Close();
            _currentWindow = null;
        }

        /// <summary>
        /// Get current active window
        /// </summary>
        public Window? GetCurrentWindow()
        {
            return _currentWindow;
        }
    }
}
