using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetraAI.Desktop.Utils;
using Newtonsoft.Json.Linq;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for interacting with Google Gemini API
    /// </summary>
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger? _logger;
        private readonly string? _apiKey;

        public GeminiService() : this(null, null, null) { }

        public GeminiService(HttpClient? httpClient, ILogger? logger = null, string? apiKey = null)
        {
            _httpClient = httpClient ?? new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(Constants.ApiCallTimeout)
            };
            _logger = logger;
            _apiKey = apiKey;
        }

        public async Task<string> GenerateAsync(string prompt, byte[]? pngBytes, CancellationToken cancellationToken)
        {
            var apiKey = _apiKey ?? ConfigurationManager.GetValue("Gemini:ApiKey");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger?.Warning("Gemini API key is missing");
                throw new InvalidOperationException("Gemini API key is missing. Set Gemini:ApiKey in appsettings.json.");
            }

            var model = ConfigurationManager.GetValue("Gemini:Model") ?? "gemini-1.5-flash";
            var endpoint = $"{Constants.GeminiApiEndpoint}/models/{model}:generateContent?key={apiKey}";

            _logger?.Info($"Sending generation request to Gemini API (model: {model})");

            var parts = new JArray
            {
                new JObject(new JProperty("text", string.IsNullOrWhiteSpace(prompt) ? "Describe what is on my screen." : prompt))
            };

            if (pngBytes != null && pngBytes.Length > 0)
            {
                parts.Add(
                    new JObject(
                        new JProperty("inlineData", new JObject(
                            new JProperty("mimeType", "image/png"),
                            new JProperty("data", Convert.ToBase64String(pngBytes))
                        ))
                    )
                );
            }

            var maxOutputTokens = int.TryParse(ConfigurationManager.GetValue("Gemini:MaxOutputTokens"), out var maxTokens)
                ? maxTokens
                : 2048;

            var requestBody = new JObject(
                new JProperty("contents", new JArray(
                    new JObject(
                        new JProperty("parts", parts)
                    )
                )),
                new JProperty("generationConfig", new JObject(
                    new JProperty("temperature", 0.2),
                    new JProperty("maxOutputTokens", maxOutputTokens)
                ))
            );

            using var content = new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger?.Error($"Gemini API error ({(int)response.StatusCode}): {responseText}");
                throw new InvalidOperationException($"Gemini API error ({(int)response.StatusCode}): {responseText}");
            }

            var json = JObject.Parse(responseText);
            var text = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            return string.IsNullOrWhiteSpace(text) ? "No response from Gemini." : text;
        }
    }
}
