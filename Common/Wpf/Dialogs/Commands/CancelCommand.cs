using System;
using System.Windows;
using System.Windows.Input;
using Utilities;

namespace Wpf.Dialogs.Commands
{
    internal class CancelCommand : ICommand
    {
        private FileDialogViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public CancelCommand(FileDialogViewModel viewModel)
        {
            Safeguard.EnsureNotNull("viewModel", viewModel);
            this.viewModel = viewModel;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.viewModel.Result = FileDialogResultEnum.Cancel;

            Window window = parameter as Window;
            window.Close();
        }
    }
}
