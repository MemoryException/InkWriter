using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public abstract class ButtonPopupCommand : ICommand
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        protected abstract Func<bool> CanExecuteFunction { get; }

        protected abstract Func<MainWindowViewModel, Window, UserControl> ViewFunction { get; }

        public event EventHandler CanExecuteChanged;


        public ButtonPopupCommand(MainWindowViewModel mainWindowViewModel)
        {
            Safeguard.EnsureNotNull("mainWindowViewModel", mainWindowViewModel);

            this.mainWindowViewModel = mainWindowViewModel;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return this.CanExecuteFunction.Invoke();
        }

        public void Execute(object parameter)
        {
            Button button = parameter as Button;
            if (button == null)
            {
                return;
            }

            Window window = new Window
            {
                Opacity = 0
            };

            Point upperLeftPoint = button.PointToScreen(new Point(0, button.Height));
            PresentationSource presentationSource = PresentationSource.FromVisual(button);
            double dpiX = presentationSource.CompositionTarget.TransformToDevice.M11;
            double dpiY = presentationSource.CompositionTarget.TransformToDevice.M22;
            window.Left = upperLeftPoint.X / dpiX;
            window.Top = upperLeftPoint.Y / dpiY;

            if (button.Parent is StackPanel parentStackPanel)
            {
                Point rightPoint = parentStackPanel.PointToScreen(new Point(parentStackPanel.ActualWidth, 0));
                double rightBorder = rightPoint.X / dpiX;
                window.MaxWidth = rightBorder - window.Left;
            }

            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Topmost = true;

            UserControl view = this.ViewFunction.Invoke(this.mainWindowViewModel, window);
            window.Content = view;
            window.WindowState = WindowState.Normal;
            window.Show();

            window.Fade(0, 1, TimeSpan.FromSeconds(0.15), null);
        }
    }
}
