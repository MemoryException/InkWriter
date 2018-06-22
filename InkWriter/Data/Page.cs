using InkWriter.Data.EventHandler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Wpf;

namespace InkWriter.Data
{
    public class Page
    {
        public Page(InkWriterDocument document)
        {
            this.PageModified += document.OnPageModified;
        }

        public Page()
        {

        }

        internal BitmapSource GetBitmap(Orientation? orientation)
        {
            InkCanvas inkCanvas = new InkCanvas
            {
                Background = System.Windows.Media.Brushes.White
            };

            if (this.StrokeData != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(this.StrokeData, 0, this.StrokeData.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    StrokeCollection collection = new StrokeCollection(memoryStream);
                    inkCanvas.Strokes = collection;
                }
            }

            foreach (Stroke stroke in inkCanvas.Strokes)
            {
                stroke.DrawingAttributes.Color = stroke.DrawingAttributes.Color == Colors.White ? Colors.Black : stroke.DrawingAttributes.Color == Colors.LightGray ? Colors.Black : stroke.DrawingAttributes.Color;
            }

            System.Windows.Point bottomRightCorner = new System.Windows.Point(1, 1);
            foreach (Stroke stroke in inkCanvas.Strokes)
            {
                Rect bounds = stroke.GetBounds();
                if (bounds.Right > bottomRightCorner.X)
                {
                    bottomRightCorner.X = bounds.Right + 1;
                }

                if (bounds.Bottom > bottomRightCorner.Y)
                {
                    bottomRightCorner.Y = bounds.Bottom + 1;
                }
            }

            double screenDpi = DpiUtilities.DesktopDpiX;
            double desiredDpi = 300;
            double scale = desiredDpi / screenDpi;

            inkCanvas.Width = bottomRightCorner.X * scale;
            inkCanvas.Height = bottomRightCorner.Y * scale;

            inkCanvas.RenderTransform = new ScaleTransform(scale, scale);
            inkCanvas.Arrange(new Rect(0, 0, bottomRightCorner.X, bottomRightCorner.Y));
            inkCanvas.UpdateLayout();

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int)inkCanvas.RenderSize.Width,
                (int)inkCanvas.RenderSize.Height,
                screenDpi,
                screenDpi,
                PixelFormats.Pbgra32);

            bitmap.Render(inkCanvas);
            bitmap.Freeze();

            if (bottomRightCorner.X > bottomRightCorner.Y && orientation.GetValueOrDefault(Orientation.Horizontal) == Orientation.Vertical
                || bottomRightCorner.Y > bottomRightCorner.X && orientation.GetValueOrDefault(Orientation.Horizontal) == Orientation.Horizontal)
            {
                return new TransformedBitmap(bitmap, new RotateTransform(90));
            }

            return bitmap;
        }

        public void SaveAsJpg(string fileName)
        {
            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
            jpg.Frames.Add(BitmapFrame.Create(this.GetBitmap(Orientation.Horizontal)));
            using (Stream stm = File.Create(fileName))
            {
                jpg.Save(stm);
            }
        }

        private Bitmap rotateImage90(Bitmap b)
        {
            Bitmap returnBitmap = new Bitmap(b.Height, b.Width);
            Graphics g = Graphics.FromImage(returnBitmap);
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            g.RotateTransform(90);
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            g.DrawImage(b, new System.Drawing.Point(0, 0));
            return returnBitmap;
        }

        public event EventHandler<PageModifiedEventArgs> PageModified;

        [XmlElement("StrokeData")]
        public byte[] StrokeData { get; set; }

        [XmlElement("Children")]
        public List<Child> Children { get; set; }

        public void SetStrokeData(byte[] buffer)
        {
            if (buffer == null)
            {
                this.StrokeData = new byte[0];
            }
            else
            {
                this.StrokeData = new byte[buffer.Length];
                buffer.CopyTo(this.StrokeData, 0);
            }

            this.PageModified?.Invoke(this, new PageModifiedEventArgs(this));
        }

        public void SetChildren(UIElementCollection children)
        {
            if (this.Children == null)
            {
                this.Children = new List<Child>();
            }

            this.Children.Clear();
            foreach (UIElement uiElement in children)
            {
                this.Children.Add(new Child(uiElement));
            }

            this.PageModified?.Invoke(this, new PageModifiedEventArgs(this));
        }

        public IEnumerable<UIElement> CreateChildren()
        {
            if (this.Children == null)
            {
                return new List<UIElement>();
            }

            List<UIElement> childrenList = new List<UIElement>();
            foreach (Child child in this.Children)
            {
                childrenList.Add(child.CreateUiElement());
            }

            return childrenList;
        }
    }
}
