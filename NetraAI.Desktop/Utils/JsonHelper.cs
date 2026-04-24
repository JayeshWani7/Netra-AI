using System;
using System.IO;
using Newtonsoft.Json;

namespace NetraAI.Desktop.Utils
{
    /// <summary>
    /// JSON helper utilities for serialization/deserialization
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Serialize object to JSON string
        /// </summary>
        public static string Serialize<T>(T obj) where T : class
        {
            try
            {
                return JsonConvert.SerializeObject(obj, _settings);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"JSON serialization failed: {ex.Message}", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserialize JSON string to object
        /// </summary>
        public static T? Deserialize<T>(string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, _settings);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"JSON deserialization failed: {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// Deserialize JSON from file
        /// </summary>
        public static T? DeserializeFromFile<T>(string filePath) where T : class
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var json = File.ReadAllText(filePath);
                return Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to deserialize from file: {ex.Message}", ex);
                return null;
            }
        }

        /// <summary>
        /// Serialize to JSON file
        /// </summary>
        public static bool SerializeToFile<T>(T obj, string filePath) where T : class
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
                var json = Serialize(obj);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to serialize to file: {ex.Message}", ex);
                return false;
            }
        }
    }
}
