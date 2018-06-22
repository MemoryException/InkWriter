using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using Utilities;

namespace InkWriter.Data
{
    public enum ChildType
    {
        Undefined = 0,
        Image
    }

    public class Child
    {
        [XmlElement("ChildType")]
        public ChildType ChildType { get; set; }

        [XmlElement("Data")]
        public byte[] Data { get; set; }

        [XmlElement("Width")]
        public double Width { get; set; }
        
        [XmlElement("Height")]
        public double Height { get; set; }

        [XmlElement("Left")]
        public double Left { get; set; }

        [XmlElement("Top")]
        public double Top { get; set; }

        public Child(UIElement uiElement)
        {
            Safeguard.EnsureNotNull("uiElement", uiElement);

            if (uiElement is Image)
            {
                this.ChildType = ChildType.Image;

                Image image = uiElement as Image;
                this.Width = image.RenderSize.Width;
                this.Height = image.RenderSize.Height;

                UIElement container = VisualTreeHelper.GetParent(uiElement) as UIElement;
                Point relativeLocation = uiElement.TranslatePoint(new Point(0, 0), container);
                this.Left = relativeLocation.X;
                this.Top = relativeLocation.Y;

                this.Data = ByteImageConverter.ConvertBitmapSourceToByteArray(image.Source);
                return;
            }

            throw new NotSupportedException("Type " + uiElement.GetType() + " not supported.");
        }

        public Child()
        {
            this.ChildType = ChildType.Undefined;
        }

        public UIElement CreateUiElement()
        {
            switch (this.ChildType)
            {
                case ChildType.Image:
                    Image image = new Image();
                    image.Width = this.Width;
                    image.Height = this.Height;

                    InkCanvas.SetLeft(image, this.Left);
                    InkCanvas.SetTop(image, this.Top);

                    image.Source = ByteImageConverter.ConvertByteArrayToBitmapImage(this.Data);
                    return image;
                default:
                    throw new NotSupportedException("Child type " + this.ChildType + " not supported.");
            }
        }
    }
}
