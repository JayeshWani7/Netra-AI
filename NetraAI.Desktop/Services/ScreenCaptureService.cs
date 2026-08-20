using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using NetraAI.Desktop.Utils;

namespace NetraAI.Desktop.Services
{
    /// <summary>
    /// Service for taking primary screen and region-based screen captures
    /// </summary>
    public class ScreenCaptureService
    {
        /// <summary>
        /// Captures the full primary screen as a PNG byte array
        /// </summary>
        public byte[] CapturePrimaryScreenPng()
        {
            var width = (int)Math.Max(1, SystemParameters.PrimaryScreenWidth);
            var height = (int)Math.Max(1, SystemParameters.PrimaryScreenHeight);
            return CaptureRegionPng(0, 0, width, height);
        }

        /// <summary>
        /// Captures a specific rectangular screen region as a PNG byte array
        /// </summary>
        public byte[] CaptureRegionPng(int x, int y, int width, int height)
        {
            try
            {
                var validWidth = Math.Max(1, width);
                var validHeight = Math.Max(1, height);

                using var bitmap = new Bitmap(validWidth, validHeight, PixelFormat.Format32bppArgb);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(validWidth, validHeight), CopyPixelOperation.SourceCopy);

                using var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Screen capture failed: {ex.Message}", ex);
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Converts a PNG byte array into a WPF BitmapSource for UI image rendering
        /// </summary>
        public static System.Windows.Media.Imaging.BitmapSource? ConvertToBitmapSource(byte[]? pngBytes)
        {
            if (pngBytes == null || pngBytes.Length == 0)
                return null;

            try
            {
                using var stream = new MemoryStream(pngBytes);
                var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Failed to convert PNG bytes to BitmapSource: {ex.Message}", ex);
                return null;
            }
        }
    }
}
