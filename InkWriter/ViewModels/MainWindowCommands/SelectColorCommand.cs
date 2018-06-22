using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class SelectColorCommand : ICommand
    {
        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public SelectColorCommand(MainWindowViewModel mainWindow)
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

            Window window = new Window();
            window.Opacity = 0;
            Point upperLeftPoint = button.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            window.Left = upperLeftPoint.X;
            window.Top = upperLeftPoint.Y + 50;
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Topmost = true;
            Views.SelectColorView view = new Views.SelectColorView() { DataContext = new ViewModels.SelectColorViewModel(this.mainWindow.InkCanvas, new List<Color> { Colors.LightGray, Colors.Blue, Colors.Green, Colors.Red }, window) };
            window.Content = view;
            window.WindowState = WindowState.Normal;
            window.Show();

            window.Fade(0, 1, TimeSpan.FromSeconds(0.15), null);
        }
    }
}
