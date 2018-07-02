using InkWriter.Data;
using System;
using System.Windows.Input;
using Utilities;
using Wpf.Dialogs;

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
            SearchPattern[] searchPatterns = new SearchPattern[]
            {
                new SearchPattern("InkWriter-Dateien (*.iwd)", "*.iwd"),
                new SearchPattern("Alle Dateien (*.*)", "*.*")
            };

            FileDialogResult result = FileDialog.Show("Open File...", searchPatterns, searchPatterns[0]);

            if (result.Result == FileDialogResultEnum.OK)
            {
                this.mainWindow.SetActiveDocument(InkWriterDocument.Load(result.SelectedFile));
            }
        }
    }
}
