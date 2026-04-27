using System.Windows;
using NetraAI.Desktop.Utils;
using System.IO;
using NetraAI.Desktop.Views;
using NetraAI.Desktop.Services;

namespace NetraAI.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Write startup marker
                File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] App.OnStartup() called\n");
                
                // Initialize logging
                Logger.Initialize();
                var logger = Logger.GetInstance();
                
                File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] Logger initialized\n");
                
                // Initialize dependency injection
                ServiceProvider.Initialize();
                
                File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] DI initialized\n");
                
                logger.Info("Netra AI application started");

                File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] Creating ShellWindow\n");
                var shellWindow = new ShellWindow();
                MainWindow = shellWindow;
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                var navigationService = ServiceProvider.GetRequiredService<NavigationService>();
                navigationService.InitializeShell(shellWindow, shellWindow.GetContentHost());
                navigationService.NavigateToLogin();

                shellWindow.Show();
                shellWindow.Activate();
                File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] ShellWindow shown\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] EXCEPTION: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
                MessageBox.Show($"Failed to start login window:\n{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            File.AppendAllText(Path.Combine(Path.GetTempPath(), "netrai_startup.log"), $"[{DateTime.Now:HH:mm:ss.fff}] App.OnExit() called\n");
            try
            {
                var logger = Logger.GetInstance();
                logger.Info("Netra AI application closed");
                Logger.Close();
            }
            catch
            {
                // Logging failed
            }
            base.OnExit(e);
        }
    }
}
