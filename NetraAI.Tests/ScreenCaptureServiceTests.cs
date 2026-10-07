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

        [Fact]
        public void IsValidPngHeader_ValidPngHeader_ReturnsTrue()
        {
            var validPngHeader = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13 };
            Assert.True(ScreenCaptureService.IsValidPngHeader(validPngHeader));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new byte[0])]
        [InlineData(new byte[] { 1, 2, 3 })]
        [InlineData(new byte[] { 0, 80, 78, 71, 13, 10, 26, 10 })]
        public void IsValidPngHeader_InvalidOrShort_ReturnsFalse(byte[]? bytes)
        {
            Assert.False(ScreenCaptureService.IsValidPngHeader(bytes));
        }

        [Fact]
        public void ToBase64DataUrl_ValidBytes_ReturnsFormattedDataUrl()
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            var dataUrl = ScreenCaptureService.ToBase64DataUrl(bytes);

            Assert.StartsWith("data:image/png;base64,", dataUrl);
            Assert.Contains(System.Convert.ToBase64String(bytes), dataUrl);
        }

        [Fact]
        public void ToBase64DataUrl_NullOrEmpty_ReturnsEmptyString()
        {
            Assert.Equal(string.Empty, ScreenCaptureService.ToBase64DataUrl(null));
            Assert.Equal(string.Empty, ScreenCaptureService.ToBase64DataUrl(System.Array.Empty<byte>()));
        }

        [Fact]
        public void TrySaveToFile_ValidBytes_SavesFileSuccessfully()
        {
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"netra_test_{System.Guid.NewGuid()}.png");
            try
            {
                var bytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
                var result = ScreenCaptureService.TrySaveToFile(bytes, tempPath);

                Assert.True(result);
                Assert.True(System.IO.File.Exists(tempPath));
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
        }

        [Fact]
        public void TrySaveToFile_NullOrEmptyBytes_ReturnsFalse()
        {
            Assert.False(ScreenCaptureService.TrySaveToFile(null, "some/path.png"));
            Assert.False(ScreenCaptureService.TrySaveToFile(System.Array.Empty<byte>(), "some/path.png"));
            Assert.False(ScreenCaptureService.TrySaveToFile(new byte[] { 1, 2 }, ""));
        }
    }
}
