using InkWriter.Data.EventHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

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

        [XmlIgnore()]
        public PageContent PageContent
        {
            get
            {
                PageContent pageContent = new PageContent();
                FixedPage fixedPage = new FixedPage();
                fixedPage.Children.Add(this.Image);
                pageContent.Child = fixedPage;

                return pageContent;
            }
        }

        [XmlIgnore()]
        public Image Image
        {
            get
            {
                RenderTargetBitmap bitmap = this.GetBitmap();
                Image image = new Image();
                image.HorizontalAlignment = HorizontalAlignment.Stretch;
                image.VerticalAlignment = VerticalAlignment.Stretch;
                image.Source = bitmap;
                return image;
            }
        }

        internal RenderTargetBitmap GetBitmap()
        {
            InkCanvas inkCanvas = new InkCanvas
            {
                Background = Brushes.White
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

            Point bottomRightCorner = new Point(0, 0);
            foreach (Stroke stroke in inkCanvas.Strokes)
            {
                Rect bounds = stroke.GetBounds();
                if (bounds.Right > bottomRightCorner.X)
                {
                    bottomRightCorner.X = bounds.Right;
                }

                if (bounds.Bottom > bottomRightCorner.Y)
                {
                    bottomRightCorner.Y = bounds.Bottom;
                }
            }

            double scaleX = bottomRightCorner.X > 816 ? 816 / bottomRightCorner.X : 1;
            double scaleY = bottomRightCorner.Y > 1056 ? 1056 / bottomRightCorner.Y : 1;
            double scale = scaleX < scaleY ? scaleX : scaleY;

            inkCanvas.LayoutTransform = new ScaleTransform(scale, scale);

            inkCanvas.Measure(new Size(bottomRightCorner.X, bottomRightCorner.Y));
            inkCanvas.Arrange(new Rect(new Size(bottomRightCorner.X, bottomRightCorner.Y)));

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)bottomRightCorner.X * 8 + 50, (int)bottomRightCorner.Y * 8 + 50, 384 * 2, 384 * 2, PixelFormats.Pbgra32);
            bitmap.Render(inkCanvas);
            return bitmap;
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
