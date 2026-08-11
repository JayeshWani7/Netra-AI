using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

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
            var validWidth = Math.Max(1, width);
            var validHeight = Math.Max(1, height);

            using var bitmap = new Bitmap(validWidth, validHeight, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(validWidth, validHeight), CopyPixelOperation.SourceCopy);

            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
