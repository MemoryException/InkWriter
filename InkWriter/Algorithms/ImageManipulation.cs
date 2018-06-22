using System;
using System.Drawing;

namespace InkWriter.Algorithms
{
    public class ImageManipulation
    {
        public static Bitmap Copy(Bitmap sourceBitmap)
        {
            Bitmap targetBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            FastBitmap target = new FastBitmap(targetBitmap);
            target.LockImage();

            FastBitmap source = new FastBitmap(sourceBitmap);
            source.LockImage();

            for (int x = 0; x < sourceBitmap.Width; ++x)
                for (int y = 0; y < sourceBitmap.Height; ++y)
                {
                    target.SetPixel(x, y, source.GetPixel(x, y));
                }

            target.UnlockImage();
            source.UnlockImage();

            return targetBitmap;
        }

        public static Bitmap Cut(Bitmap sourceBitmap, PointF topLeft, PointF topRight, PointF bottomLeft, PointF bottomRight)
        {
            Vector leftVector = new Vector(topLeft, bottomLeft);
            Vector rightVector = new Vector(topRight, bottomRight);
            Vector topVector = new Vector(topLeft, topRight);
            Vector bottomVector = new Vector(bottomLeft, bottomRight);

            int width = (int)Math.Round(Math.Max(topVector.Length, bottomVector.Length), 0);
            int height = (int)Math.Round(Math.Max(leftVector.Length, rightVector.Length), 0);

            Bitmap targetBitmap = new Bitmap(width, height);
            FastBitmap target = new FastBitmap(targetBitmap);
            target.LockImage();

            FastBitmap source = new FastBitmap(sourceBitmap);
            source.LockImage();

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    float relativeX = x / (float)width;
                    float relativeY = y / (float)height;

                    Vector horizontalVector = leftVector.Merge(rightVector, relativeX);
                    Vector verticalVector = topVector.Merge(bottomVector, relativeY);

                    PointF intersection = horizontalVector.Intersect(verticalVector);

                    if (intersection != PointF.Empty && intersection.X < sourceBitmap.Width && intersection.Y < sourceBitmap.Height)
                    {
                        target.SetPixel(x, y, source.GetPixel((int)intersection.X, (int)intersection.Y));
                    }
                }

            target.UnlockImage();
            source.UnlockImage();

            return targetBitmap;
        }
    }
}