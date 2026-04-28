using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetraAI.Desktop.Utils;
using Newtonsoft.Json.Linq;

namespace NetraAI.Desktop.Services
{
    public class GeminiService
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(Constants.ApiCallTimeout)
        };

        public async Task<string> GenerateAsync(string prompt, byte[]? pngBytes, CancellationToken cancellationToken)
        {
            var apiKey = ConfigurationManager.GetValue("Gemini:ApiKey");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Gemini API key is missing. Set Gemini:ApiKey in appsettings.json.");
            }

            var model = ConfigurationManager.GetValue("Gemini:Model") ?? "gemini-1.5-flash";
            var endpoint = $"{Constants.GeminiApiEndpoint}/models/{model}:generateContent?key={apiKey}";

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
            using var response = await HttpClient.PostAsync(endpoint, content, cancellationToken);
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Gemini API error ({(int)response.StatusCode}): {responseText}");
            }

            var json = JObject.Parse(responseText);
            var text = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            return string.IsNullOrWhiteSpace(text) ? "No response from Gemini." : text;
        }
    }
}
