using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace InkWriter.Algorithms
{
    unsafe public class FastBitmap
    {
        private struct PixelData
        {
            public byte blue;
            public byte green;
            public byte red;
            public byte alpha;
        }

        private Bitmap workingBitmap = null;
        private int width = 0;
        private BitmapData bitmapData = null;
        private Byte* basePointer = null;

        public FastBitmap(Bitmap inputBitmap)
        {
            workingBitmap = inputBitmap;
        }

        public void LockImage()
        {
            Rectangle bounds = new Rectangle(Point.Empty, workingBitmap.Size);

            width = (int)(bounds.Width * sizeof(PixelData));
            if (width % 4 != 0) width = 4 * (width / 4 + 1);

            bitmapData = workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            basePointer = (Byte*)bitmapData.Scan0.ToPointer();
        }

        private PixelData* pixelData = null;

        public Color GetPixel(int left, int top)
        {
            pixelData = (PixelData*)(basePointer + top * width + left * sizeof(PixelData));
            return Color.FromArgb(pixelData->alpha, pixelData->red, pixelData->green, pixelData->blue);
        }

        public Color GetPixelNext()
        {
            pixelData++;
            return Color.FromArgb(pixelData->alpha, pixelData->red, pixelData->green, pixelData->blue);
        }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData* data = (PixelData*)(basePointer + y * width + x * sizeof(PixelData));
            data->alpha = color.A;
            data->green = color.G;
            data->blue = color.B;
            data->red = color.R;
        }

        public void UnlockImage()
        {
            workingBitmap.UnlockBits(bitmapData);
            bitmapData = null;
            basePointer = null;
        }
    }
}
