using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace InkWriter.Behaviors
{
    public class ReflectDimensionsBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ElementWidthProperty = DependencyProperty.Register("ElementWidth", typeof(double), typeof(ReflectDimensionsBehavior), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ElementHeightProperty = DependencyProperty.Register("ElementHeight", typeof(double), typeof(ReflectDimensionsBehavior), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double ElementWidth
        {
            get { return (double)GetValue(ElementWidthProperty); }
            set { SetValue(ElementWidthProperty, value); }
        }

        public double ElementHeight
        {
            get { return (double)GetValue(ElementHeightProperty); }
            set { SetValue(ElementHeightProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        protected virtual void OnLoaded()
        {
            this.AssociatedObject.LayoutUpdated += (sender, e) =>
            {
                this.ElementWidth = this.AssociatedObject.ActualWidth;
                this.ElementHeight = this.AssociatedObject.ActualHeight;
            };
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoaded();
        }
    }
}