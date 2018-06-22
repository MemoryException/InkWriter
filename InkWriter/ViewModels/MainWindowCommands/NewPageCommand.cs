using InkWriter.Data;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class NewPageCommand : ICommand
    {
        private InkWriterDocument document;
        private MainWindowViewModel mainWindow;
        public event EventHandler CanExecuteChanged;

        public NewPageCommand(MainWindowViewModel mainWindow, InkWriterDocument document)
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

                this.document.Pages.Insert(this.document.ActivePageIndex + 1, new Data.Page(this.document));
                this.document.ActivePageIndex = this.document.ActivePageIndex + 1;

                this.mainWindow.FadeIn(null);
            }));
        }
    }
}
