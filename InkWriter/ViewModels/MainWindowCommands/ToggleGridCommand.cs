using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class ToggleGridCommand : ICommand
    {
        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public ToggleGridCommand(MainWindowViewModel mainWindow)
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
            this.mainWindow.ShowGrid = !this.mainWindow.ShowGrid;
        }
    }
}
