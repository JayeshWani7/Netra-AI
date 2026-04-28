using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace NetraAI.Desktop.Services
{
    public class ScreenCaptureService
    {
        public byte[] CapturePrimaryScreenPng()
        {
            var width = (int)Math.Max(1, SystemParameters.PrimaryScreenWidth);
            var height = (int)Math.Max(1, SystemParameters.PrimaryScreenHeight);
            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);

            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
