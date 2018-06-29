using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wpf
{
    public class GridCanvas : Grid
    {
        public static readonly DependencyProperty GridTypeProperty = DependencyProperty.Register("GridType", typeof(GridType), typeof(GridCanvas), new FrameworkPropertyMetadata(GridType.None, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridSizeProperty = DependencyProperty.Register("GridSize", typeof(double), typeof(GridCanvas), new FrameworkPropertyMetadata((double)50, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridColorProperty = DependencyProperty.Register("GridColor", typeof(SolidColorBrush), typeof(GridCanvas), new FrameworkPropertyMetadata(Brushes.DarkSlateGray, FrameworkPropertyMetadataOptions.AffectsRender));

        private List<UIElement> gridLines = new List<UIElement>();

        private GridType renderedGridType;

        private double renderedGridSize;

        private SolidColorBrush renderedGridColor;

        public GridCanvas() : base()
        {
            this.SizeChanged += this.OnSizeChanged;
            this.renderedGridType = this.GridType;
            this.renderedGridSize = this.GridSize;
            this.renderedGridColor = this.GridColor;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double height = e.NewSize.Height;
            this.RebuildGrid(width, height);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.GridType != this.renderedGridType || this.GridSize != this.renderedGridSize || this.GridColor != this.renderedGridColor)
            {
                this.RebuildGrid();
            }

            base.OnRender(dc);
        }

        private void RebuildGrid(double width, double height)
        {
            foreach (var gridLine in this.gridLines)
            {
                this.Children.Remove(gridLine);
            }

            this.gridLines.Clear();
            switch (this.GridType)
            {
                case GridType.None:
                    break;
                case GridType.Lined:
                    this.BuildVerticalLines(width, height);
                    break;
                case GridType.Checked:
                    this.BuildVerticalLines(width, height);
                    this.BuildHorizontalLines(width, height);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("gridType", "Grid type is out of range.");
            }

            this.renderedGridType = this.GridType;
            this.renderedGridSize = this.GridSize;
            this.renderedGridColor = this.GridColor;
        }

        private void BuildHorizontalLines(double width, double height)
        {
            double currentLeft = this.GridSize;
            while (currentLeft < width)
            {
                Line newLine = new Line()
                {
                    X1 = currentLeft,
                    X2 = currentLeft,
                    Y1 = 0,
                    Y2 = height,
                    Stroke = Brushes.DarkSlateGray
                };

                this.Children.Add(newLine);
                this.gridLines.Add(newLine);

                currentLeft += this.GridSize;
            }
        }

        private void BuildVerticalLines(double width, double height)
        {
            double currentHeight = this.GridSize;
            while (currentHeight < height)
            {
                Line newLine = new Line()
                {
                    X1 = 0,
                    X2 = width,
                    Y1 = currentHeight,
                    Y2 = currentHeight,
                    Stroke = Brushes.DarkSlateGray
                };

                this.Children.Add(newLine);
                this.gridLines.Add(newLine);

                currentHeight += this.GridSize;
            }
        }

        public GridType GridType
        {
            get { return (GridType)GetValue(GridTypeProperty); }
            set
            {
                SetValue(GridTypeProperty, value);
                this.RebuildGrid();
            }
        }

        public double GridSize
        {
            get { return (double)GetValue(GridSizeProperty); }
            set
            {
                SetValue(GridSizeProperty, value);
                this.RebuildGrid();
            }
        }

        public SolidColorBrush GridColor
        {
            get { return (SolidColorBrush)GetValue(GridColorProperty); }
            set
            {
                SetValue(GridColorProperty, value);
                this.RebuildGrid();
            }
        }

        private void RebuildGrid()
        {
            if (!double.IsNaN(this.ActualWidth) && !double.IsNaN(this.ActualHeight))
            {
                this.RebuildGrid(this.ActualWidth, this.ActualHeight);
            }
        }
    }
}
