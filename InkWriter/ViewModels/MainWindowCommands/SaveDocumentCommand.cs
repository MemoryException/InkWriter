using InkWriter.Data;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class SaveDocumentCommand : ICommand
    {
        private InkWriterDocument document;

        public event EventHandler CanExecuteChanged;

        public SaveDocumentCommand(InkWriterDocument document)
        {
            Safeguard.EnsureNotNull("document", document);

            this.document = document;
        }

        public void ResetDocument(InkWriterDocument newDocument)
        {
            Safeguard.EnsureNotNull("newDocument", newDocument);

            this.document = newDocument;
        }

        public bool CanExecute(object parameter)
        {
            return this.document != null;
        }

        public void Execute(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save document...";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.InitialDirectory = !string.IsNullOrEmpty(document.FilePath) ? Path.GetDirectoryName(document.FilePath) : string.Empty;
            saveFileDialog.FileName = Path.GetFileName(document.FilePath);
            saveFileDialog.DefaultExt = "iwd";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "InkWriterDocuments (*.iwd)|*.iwd";
            if (saveFileDialog.ShowDialog() == true)
            {
                this.document.Save(saveFileDialog.FileName);
            }
        }
    }
}
