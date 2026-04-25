using Newtonsoft.Json;

namespace NetraAI.Desktop.Models
{
    /// <summary>
    /// Firebase configuration model
    /// </summary>
    public class FirebaseConfig
    {
        [JsonProperty("ApiKey")]
        public string ApiKey { get; set; } = string.Empty;

        [JsonProperty("AuthDomain")]
        public string AuthDomain { get; set; } = string.Empty;

        [JsonProperty("ProjectId")]
        public string ProjectId { get; set; } = string.Empty;

        [JsonProperty("StorageBucket")]
        public string StorageBucket { get; set; } = string.Empty;

        [JsonProperty("MessagingSenderId")]
        public string MessagingSenderId { get; set; } = string.Empty;

        [JsonProperty("AppId")]
        public string AppId { get; set; } = string.Empty;

        [JsonProperty("MeasurementId")]
        public string? MeasurementId { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ApiKey) &&
                   !string.IsNullOrEmpty(AuthDomain) &&
                   !string.IsNullOrEmpty(ProjectId) &&
                   !string.IsNullOrEmpty(StorageBucket) &&
                   !string.IsNullOrEmpty(MessagingSenderId) &&
                   !string.IsNullOrEmpty(AppId);
        }
    }
}
