using System;
using System.Windows;
using System.Windows.Media.Animation;
using Utilities;

namespace Wpf
{
    public static class Animations
    {
        public static void Fade(this FrameworkElement element, double startOpacity, double toOpacity, TimeSpan timeSpan, EventHandler completedEvent)
        {
            Safeguard.EnsureNotNull("element", element);

            DoubleAnimation fadeAnimation = new DoubleAnimation()
            {
                From = startOpacity,
                To = toOpacity,
                Duration = timeSpan
            };

            Storyboard.SetTarget(fadeAnimation, element);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(UIElement.OpacityProperty));
            
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(fadeAnimation);

            if (completedEvent != null)
            {
                storyboard.Completed += completedEvent;
            }

            storyboard.Begin();
        }
    }
}