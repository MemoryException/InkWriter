using InkWriter.Data;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class CreateNewDocumentCommand : ICommand
    {
        private InkWriterDocument document;
        private MainWindowViewModel mainWindow;
        public event EventHandler CanExecuteChanged;

        public CreateNewDocumentCommand(MainWindowViewModel mainWindow, InkWriterDocument document)
        {
            Safeguard.EnsureNotNull("mainWindow", mainWindow);
            Safeguard.EnsureNotNull("document", document);

            this.mainWindow = mainWindow;
            this.document = document;

            this.CanExecuteChanged?.Invoke(this, new EventArgs());
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
            this.mainWindow.FadeOut(new EventHandler(delegate (object sender, EventArgs e)
            {
                this.mainWindow.ResetScale();

                int selectedPageIndex = this.document.ActivePageIndex;
                this.document.Clear();
                this.document.ActivePageIndex = 0;

                this.mainWindow.AutoScale();

                this.mainWindow.FadeIn(null);
            }));

        }
    }
}
