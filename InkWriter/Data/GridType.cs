using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InkWriter.Data
{
    public enum GridTypeEnum
    {
        Undefined = 0,
        None,
        Lined,
        Checked
    }

    public static class GridType
    {
        private static Dictionary<int, GridTypeEnum> GridTypes = new Dictionary<int, GridTypeEnum>();

        public static GridTypeEnum Next(this GridTypeEnum gridType)
        {
            switch (gridType)
            {
                case GridTypeEnum.None:
                    return GridTypeEnum.Lined;
                case GridTypeEnum.Lined:
                    return GridTypeEnum.Checked;
                case GridTypeEnum.Checked:
                    return GridTypeEnum.None;
                default:
                    throw new ArgumentOutOfRangeException("gridType", "Grid type is out of range.");
            }
        }

        public static Canvas CreateCanvas()
        {
            Canvas returnCanvas = new Canvas();
            returnCanvas.SizeChanged += GridType.SizeChanged;
            return returnCanvas;
        }

        private static void SizeChanged(object sender, EventArgs e)
        {
            Canvas senderCanvas = sender as Canvas;
            if (senderCanvas == null)
            {
                return;
            }

            UpdateCanvas(GridTypes[senderCanvas.GetHashCode()], senderCanvas);
        }

        public static void UpdateCanvas(this GridTypeEnum gridType, Canvas canvas)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException("canvas", "Canvas must not be null.");
            }

            int hash = canvas.GetHashCode();

            if (!GridTypes.ContainsKey(hash))
            {
                GridTypes.Add(hash, GridTypeEnum.None);
            }

            canvas.Children.Clear();

            if (!double.IsNaN(canvas.ActualWidth) && !double.IsNaN(canvas.ActualHeight))
            {
                switch (gridType)
                {
                    case GridTypeEnum.None:
                        GridTypes[hash] = GridTypeEnum.None;
                        break;
                    case GridTypeEnum.Lined:
                        GridTypes[hash] = GridTypeEnum.Lined;
                        BuildVerticalLines(canvas, canvas.ActualWidth, canvas.ActualHeight);
                        break;
                    case GridTypeEnum.Checked:
                        GridTypes[hash] = GridTypeEnum.Checked;
                        BuildVerticalLines(canvas, canvas.ActualWidth, canvas.ActualHeight);
                        BuildHorizontalLines(canvas, canvas.ActualWidth, canvas.ActualHeight);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("gridType", "Grid type is out of range.");
                }
            }

            canvas.InvalidateVisual();
            return;
        }

        private static void BuildHorizontalLines(Canvas returnCanvas, double width, double height)
        {
            double distance = 50;

            double currentLeft = distance;
            while (currentLeft < width)
            {
                returnCanvas.Children.Add(new Line()
                {
                    X1 = currentLeft,
                    X2 = currentLeft,
                    Y1 = 0,
                    Y2 = height,
                    Stroke = Brushes.DarkSlateGray
                });

                currentLeft += distance;
            }
        }

        private static void BuildVerticalLines(Canvas returnCanvas, double width, double height)
        {
            double distance = 50;

            double currentHeight = distance;
            while (currentHeight < height)
            {
                returnCanvas.Children.Add(new Line()
                {
                    X1 = 0,
                    X2 = width,
                    Y1 = currentHeight,
                    Y2 = currentHeight,
                    Stroke = Brushes.DarkSlateGray
                });

                currentHeight += distance;
            }
        }
    }
}
