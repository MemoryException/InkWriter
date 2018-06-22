using InkWriter.Data;
using Microsoft.Win32;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class LoadDocumentCommand : ICommand
    {
        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public LoadDocumentCommand(MainWindowViewModel mainWindow)
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
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Load document...";
            fileDialog.Multiselect = false;
            fileDialog.RestoreDirectory = true;
            fileDialog.DefaultExt = "iwd";
            fileDialog.AddExtension = true;
            fileDialog.Filter = "InkWriterDocuments (*.iwd)|*.iwd";
            if (fileDialog.ShowDialog() == true)
            {
                this.mainWindow.SetActiveDocument(InkWriterDocument.Load(fileDialog.FileName));
            }

        }
    }
}
