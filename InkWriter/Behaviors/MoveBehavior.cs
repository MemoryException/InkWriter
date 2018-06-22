using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace InkWriter.Behaviors
{
    public class MoveBehavior : Behavior<FrameworkElement>
    {
        private IInputElement parent;
        private Point mouseStartPosition;
        private TranslateTransform transform = new TranslateTransform();
        private bool moveInProgress;

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(MoveBehavior), new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        protected virtual void OnLoaded()
        {
            this.AssociatedObject.RenderTransform = transform;
            this.parent = this.AssociatedObject.Parent as IInputElement;

            this.AssociatedObject.MouseLeftButtonDown += (sender, e) =>
            {
                mouseStartPosition = e.GetPosition(this.parent);
                AssociatedObject.CaptureMouse();
                this.moveInProgress = true;
            };

            this.AssociatedObject.MouseLeftButtonUp += (sender, e) =>
            {
                AssociatedObject.ReleaseMouseCapture();
                this.moveInProgress = false;
            };

            this.AssociatedObject.MouseMove += (sender, e) =>
            {
                if (this.moveInProgress)
                {
                    Point currentMousePosition = e.GetPosition(this.parent);
                    Vector diff = currentMousePosition - this.Position;
                    if (this.AssociatedObject.IsMouseCaptured)
                    {
                        transform.X = diff.X;
                        transform.Y = diff.Y;
                    }

                    if (this.Position != null)
                    {
                        this.Position = this.Position + diff;
                    }
                }
            };
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.OnLoaded();
        }
    }
}
