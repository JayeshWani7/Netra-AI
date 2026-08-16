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

        [JsonProperty("GoogleClientId")]
        public string? GoogleClientId { get; set; }

        [JsonProperty("GoogleClientSecret")]
        public string? GoogleClientSecret { get; set; }

        [JsonProperty("GoogleRedirectUri")]
        public string? GoogleRedirectUri { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ApiKey) &&
                   !string.IsNullOrWhiteSpace(AuthDomain) &&
                   !string.IsNullOrWhiteSpace(ProjectId) &&
                   !string.IsNullOrWhiteSpace(StorageBucket) &&
                   !string.IsNullOrWhiteSpace(MessagingSenderId) &&
                   !string.IsNullOrWhiteSpace(AppId);
        }

        public bool IsGoogleAuthConfigured()
        {
            return !string.IsNullOrWhiteSpace(GoogleClientId);
        }

        public bool IsGoogleOAuthFullyConfigured()
        {
            return IsGoogleAuthConfigured() &&
                   !string.IsNullOrWhiteSpace(GoogleClientSecret) &&
                   !string.IsNullOrWhiteSpace(GoogleRedirectUri);
        }
    }
}
