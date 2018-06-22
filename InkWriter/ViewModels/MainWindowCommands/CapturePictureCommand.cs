using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class CapturePictureCommand : ICommand
    {
        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public CapturePictureCommand(MainWindowViewModel mainWindow)
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
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.Manual;
            window.Topmost = true;
            Views.CapturePictureView view = new Views.CapturePictureView { DataContext = new CapturePictureViewModel(window, this.mainWindow) };
            window.Content = view;
            window.WindowState = WindowState.Maximized;

            window.ShowDialog();
        }
    }
}
