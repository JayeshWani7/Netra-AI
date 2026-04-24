using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for navigation between windows
    /// </summary>
    public class NavigationService
    {
        private readonly Dictionary<string, Type> _viewMap = new();
        private readonly ILogger _logger;

        public NavigationService(ILogger logger)
        {
            _logger = logger;
            RegisterViews();
        }

        private void RegisterViews()
        {
            // TODO: Register all window types
            _logger.Info("Navigation service initialized");
        }

        /// <summary>
        /// Navigate to a specific window
        /// </summary>
        public void NavigateTo(string windowName)
        {
            try
            {
                _logger.Info($"Navigating to: {windowName}");
                // TODO: Implement navigation logic
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
            // TODO: Close window
        }
    }
}
