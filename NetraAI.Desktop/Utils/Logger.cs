using System;
using System.IO;
using Serilog;
using Serilog.Core;

namespace NetraAI.Desktop.Utils
{
    /// <summary>
    /// Interface for logging
    /// </summary>
    public interface ILogger
    {
        void Info(string message);
        void Debug(string message);
        void Warning(string message);
        void Error(string message, Exception? ex = null);
        void Fatal(string message, Exception? ex = null);
    }

    /// <summary>
    /// Serilog-based logger implementation
    /// </summary>
    public class Logger : ILogger
    {
        private static Logger? _instance;
        private readonly ILogger _serilogLogger;
        private static object _lockObject = new object();

        private Logger()
        {
            _serilogLogger = this;
        }

        /// <summary>
        /// Initialize logger (call once on app startup)
        /// </summary>
        public static void Initialize()
        {
            lock (_lockObject)
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                    ConfigureSerilog();
                    _instance.Info("Logger initialized");
                }
            }
        }

        /// <summary>
        /// Get logger instance
        /// </summary>
        public static ILogger GetInstance()
        {
            if (_instance == null)
            {
                Initialize();
            }
            return _instance!;
        }

        private static void ConfigureSerilog()
        {
            try
            {
                var logsPath = Constants.LogsPath;
                Directory.CreateDirectory(logsPath);

                var logFilePath = Path.Combine(
                    logsPath,
                    string.Format(Constants.LogFilePattern, DateTime.Now)
                );

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(
                        logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: Constants.LogRetentionDays,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .WriteTo.Debug(
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Serilog: {ex.Message}");
            }
        }

        public void Info(string message)
        {
            Log.Information(message);
        }

        public void Debug(string message)
        {
            Log.Debug(message);
        }

        public void Warning(string message)
        {
            Log.Warning(message);
        }

        public void Error(string message, Exception? ex = null)
        {
            if (ex != null)
                Log.Error(ex, message);
            else
                Log.Error(message);
        }

        public void Fatal(string message, Exception? ex = null)
        {
            if (ex != null)
                Log.Fatal(ex, message);
            else
                Log.Fatal(message);
        }

        /// <summary>
        /// Close logger and flush logs
        /// </summary>
        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
