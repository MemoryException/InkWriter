using InkWriter.Data;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class DeletePageCommand : ICommand
    {
        private InkWriterDocument document;
        private MainWindowViewModel mainWindow;
        public event EventHandler CanExecuteChanged;

        public DeletePageCommand(MainWindowViewModel mainWindow, InkWriterDocument document)
        {
            Safeguard.EnsureNotNull("mainWindow", mainWindow);
            Safeguard.EnsureNotNull("document", document);

            this.mainWindow = mainWindow;
            this.document = document;
            this.document.PageChanged += Document_PageChanged;
        }

        public void ResetDocument(InkWriterDocument newDocument)
        {
            Safeguard.EnsureNotNull("newDocument", newDocument);

            this.document = newDocument;
        }

        private void Document_PageChanged(object sender, Data.EventHandler.ActivePageChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return this.document.ActivePage != null;
        }

        public void Execute(object parameter)
        {
            this.mainWindow.FadeOut(new EventHandler(delegate (object sender, EventArgs e)
            {
                this.mainWindow.ResetScale();

                int selectedPageIndex = this.document.ActivePageIndex;
                this.document.Pages.Remove(this.document.ActivePage);
                if (selectedPageIndex > this.document.Pages.Count - 1)
                {
                    this.document.ActivePageIndex = this.document.Pages.Count - 1;
                }
                else
                {
                    this.document.ActivePageIndex = selectedPageIndex;
                }

                this.mainWindow.AutoScale();

                this.mainWindow.FadeIn(null);
            }));
        }
    }
}
