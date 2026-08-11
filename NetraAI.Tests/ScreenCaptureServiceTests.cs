using Xunit;
using NetraAI.Desktop.Services;

namespace NetraAI.Tests
{
    public class ScreenCaptureServiceTests
    {
        [Fact]
        public void CapturePrimaryScreenPng_ReturnsNonEmptyBytes()
        {
            var service = new ScreenCaptureService();
            var bytes = service.CapturePrimaryScreenPng();

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void CaptureRegionPng_ValidRegion_ReturnsNonEmptyBytes()
        {
            var service = new ScreenCaptureService();
            var bytes = service.CaptureRegionPng(0, 0, 100, 100);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void CaptureRegionPng_InvalidDimensions_NormalizesAndReturnsBytes()
        {
            var service = new ScreenCaptureService();
            var bytes = service.CaptureRegionPng(0, 0, -50, 0);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }
    }
}
