using System;
using System.Windows;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class MinimizeWindowCommand : ICommand
    {
        private Window window;

        public event EventHandler CanExecuteChanged;

        public MinimizeWindowCommand(Window window)
        {
            Safeguard.EnsureNotNull("window", window);

            this.window = window;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.window.WindowState = WindowState.Minimized;
        }
    }
}
