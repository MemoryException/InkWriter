using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Utilities;

namespace Wpf.Dialogs.Commands
{
    internal class OkCommand : ICommand
    {
        private FileDialogViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public OkCommand(FileDialogViewModel viewModel)
        {
            Safeguard.EnsureNotNull("viewModel", viewModel);
            this.viewModel = viewModel;
        }
        public void RecheckExecute()
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            string filePath = this.viewModel.CurrentFile;
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }

        public void Execute(object parameter)
        {
            this.viewModel.Result = FileDialogResultEnum.OK;

            Window window = parameter as Window;
            window.Close();
        }
    }
}
