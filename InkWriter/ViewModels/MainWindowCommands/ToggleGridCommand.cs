using System;
using System.Windows.Input;
using Utilities;
using Wpf;

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
            switch (this.mainWindow.GridType)
            {
                case GridType.None:
                    this.mainWindow.GridType = GridType.Lined;
                    break;
                case GridType.Lined:
                    this.mainWindow.GridType = GridType.Checked;
                    break;
                case GridType.Checked:
                    this.mainWindow.GridType = GridType.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("gridType", "Grid type is out of range.");
            }
        }
    }
}
