using System;
using System.IO;

namespace NetraAI.Desktop.Utils
{
    /// <summary>
    /// Application-wide constants
    /// </summary>
    public static class Constants
    {
        // Application Info
        public const string AppName = "Netra AI";
        public const string AppVersion = "1.0.0";
        public const string CompanyName = "Netra AI";

        // Paths
        public static string AppDataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NetraAI"
        );

        public static string LogsPath => Path.Combine(AppDataPath, "logs");
        public static string CachePath => Path.Combine(AppDataPath, "cache");
        public static string ConfigPath => Path.Combine(AppDataPath, "config");

        // File Names
        public const string SettingsFileName = "settings.json";
        public const string PermissionsFileName = "permissions.json";
        public const string ChatHistoryFileName = "chat_history.db";
        public const string ChatHistoryFolderName = "chat_history";

        // URLs
        public const string FirebaseProjectUrl = "YOUR_FIREBASE_PROJECT_URL";
        public const string GeminiApiEndpoint = "https://generativelanguage.googleapis.com/v1";

        // Timeouts (in milliseconds)
        public const int DefaultTimeout = 30000;
        public const int ScreenCaptureTimeout = 5000;
        public const int OcrTimeout = 10000;
        public const int ApiCallTimeout = 30000;

        // Sizes
        public const int MaxScreenshotCacheSize = 50;
        public const int MaxChatHistorySessions = 100;
        public const int MaxChatHistoryMessages = 500;
        public const long MaxLogFileSizeMB = 10;

        // Logging
        public const string LogFilePattern = "netraai_{0:yyyy-MM-dd}.log";
        public const int LogRetentionDays = 7;

        // Hotkeys
        public const string DefaultHotkey = "Ctrl+Alt+A";
        public const string CaptureScreenHotkey = "Ctrl+Alt+S";
        public const string SelectRegionHotkey = "Ctrl+Alt+R";

        // UI
        public const int OverlayMinWidth = 300;
        public const int OverlayMinHeight = 400;
        public const int OverlayDefaultWidth = 400;
        public const int OverlayDefaultHeight = 500;

        // Themes
        public const string ThemeDark = "dark";
        public const string ThemeLight = "light";
    }
}
