using Xunit;
using NetraAI.Desktop.Services;

namespace NetraAI.Tests
{
    public class ScreenCaptureServiceTests
    {
        [Fact]
        public void CapturePrimaryScreenPng_ReturnsByteArray()
        {
            var service = new ScreenCaptureService();
            var bytes = service.CapturePrimaryScreenPng();

            Assert.NotNull(bytes);
        }

        [Fact]
        public void CaptureRegionPng_ValidRegion_ReturnsByteArray()
        {
            var service = new ScreenCaptureService();
            var bytes = service.CaptureRegionPng(0, 0, 100, 100);

            Assert.NotNull(bytes);
        }

        [Fact]
        public void CaptureRegionPng_InvalidDimensions_NormalizesAndReturnsByteArray()
        {
            var service = new ScreenCaptureService();
            var bytes = service.CaptureRegionPng(0, 0, -50, 0);

            Assert.NotNull(bytes);
        }

        [Fact]
        public void ConvertToBitmapSource_NullOrEmptyBytes_ReturnsNull()
        {
            Assert.Null(ScreenCaptureService.ConvertToBitmapSource(null));
            Assert.Null(ScreenCaptureService.ConvertToBitmapSource(System.Array.Empty<byte>()));
        }

        [Fact]
        public void ConvertToBitmapSource_InvalidBytes_ReturnsNull()
        {
            var invalidBytes = new byte[] { 1, 2, 3, 4, 5 };
            Assert.Null(ScreenCaptureService.ConvertToBitmapSource(invalidBytes));
        }
    }
}
