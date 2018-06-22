using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InkWriter.Converters
{
    [ValueConversion(typeof(Bitmap), typeof(BitmapSource))]
    public class BitmapToBitmapSource : IValueConverter
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Bitmap bmp = value as Bitmap;
            if (bmp == null)
            {
                return null;
            }

            using (Stream stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Seek(0, SeekOrigin.Begin);
                BitmapDecoder bdc = new BmpBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return bdc.Frames[0];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource bitmapSource = value as BitmapSource;
            if (bitmapSource == null)
            {
                return null;
            }

            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            IntPtr pointer = IntPtr.Zero;
            try
            {
                System.Drawing.Imaging.PixelFormat pixelFormat = this.GetPixelFormat(bitmapSource.Format);


                pointer = Marshal.AllocHGlobal(height * stride);
                bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), pointer, height * stride, stride);
                using (var bitmap = new Bitmap(width, height, stride, pixelFormat, pointer))
                {
                    return new Bitmap(bitmap);
                }
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        private System.Drawing.Imaging.PixelFormat GetPixelFormat(System.Windows.Media.PixelFormat format)
        {
            if (format == null)
            {
                return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            }

            switch (format.BitsPerPixel)
            {
                case 32:
                    return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                default:
                    return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            }
        }
    }
}
