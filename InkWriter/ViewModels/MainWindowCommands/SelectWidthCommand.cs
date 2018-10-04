using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class SelectWidthCommand : ICommand
    {
        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public SelectWidthCommand(MainWindowViewModel mainWindow)
        {
            Safeguard.EnsureNotNull("mainWindow", mainWindow);

            this.mainWindow = mainWindow;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
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
            Views.SelectWidthView view = new Views.SelectWidthView() { DataContext = new ViewModels.SelectWidthViewModel(this.mainWindow.InkCanvas, new List<double> { 1, 2, 3, 5, 10, 15, 25, 50, 100 }, window) };
            window.Content = view;
            window.WindowState = WindowState.Normal;
            window.Show();

            window.Fade(0, 1, TimeSpan.FromSeconds(0.15), null);
        }
    }
}
