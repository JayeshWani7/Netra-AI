using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class GeminiServiceTests
    {
        [Fact]
        public void GeminiService_DefaultConstructor_InitializesSuccessfully()
        {
            var service = new GeminiService();
            Assert.NotNull(service);
        }

        [Fact]
        public async Task GenerateAsync_UnconfiguredApiKey_ThrowsInvalidOperationException()
        {
            var mockLogger = new Mock<ILogger>();
            var service = new GeminiService(null, mockLogger.Object, apiKey: "");

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GenerateAsync("Test prompt", null, CancellationToken.None));

            mockLogger.Verify(l => l.Warning(It.Is<string>(s => s.Contains("missing"))), Times.Once);
        }

        [Fact]
        public async Task GenerateAsync_SuccessfulResponse_ReturnsParsedText()
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            var jsonResponse = "{\"candidates\":[{\"content\":{\"parts\":[{\"text\":\"Hello from Gemini\"}]}}]}";

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object);
            var service = new GeminiService(httpClient, apiKey: "valid-test-key");

            var result = await service.GenerateAsync("Test prompt", null, CancellationToken.None);
            Assert.Equal("Hello from Gemini", result);
        }
    }
}
