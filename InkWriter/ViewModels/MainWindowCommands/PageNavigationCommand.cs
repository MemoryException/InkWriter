using InkWriter.Data;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class PageNavigationCommand : ICommand
    {
        private InkWriterDocument document;

        private MainWindowViewModel mainWindow;

        private NavigationRequestType navigationRequestType;

        public event EventHandler CanExecuteChanged;

        public PageNavigationCommand(MainWindowViewModel mainWindow, InkWriterDocument document, NavigationRequestType navigationRequestType)
        {
            Safeguard.EnsureNotNull("mainWindow", mainWindow);
            Safeguard.EnsureNotNull("document", document);

            this.document = document;
            this.mainWindow = mainWindow;
            this.document.PageChanged += this.OnDocumentPageChanged;
            this.navigationRequestType = navigationRequestType;
        }

        public void ResetDocument(InkWriterDocument newDocument)
        {
            Safeguard.EnsureNotNull("newDocument", newDocument);

            if (this.document != null)
            {
                this.document.PageChanged -= this.OnDocumentPageChanged;
            }

            this.document = newDocument;
            this.document.PageChanged += this.OnDocumentPageChanged;
        }

        private void OnDocumentPageChanged(object sender, Data.EventHandler.ActivePageChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            if (this.document == null)
            {
                return false;
            }

            switch (this.navigationRequestType)
            {
                case NavigationRequestType.First:
                case NavigationRequestType.Last:
                    return this.document.Pages.Count > 0;
                case NavigationRequestType.Previous:
                    return this.document.ActivePageIndex > 0;
                case NavigationRequestType.Next:
                    return this.document.ActivePageIndex < this.document.Pages.Count - 1;
                case NavigationRequestType.Absolute:
                    return true;
                default:
                    throw new InvalidOperationException("Navigation request type is out of range: " + this.navigationRequestType);
            }
        }

        public void Execute(object parameter)
        {
            this.mainWindow.FadeOut(new EventHandler(delegate (object sender, EventArgs e)
            {
                this.mainWindow.ResetScale();

                switch (this.navigationRequestType)
                {
                    case NavigationRequestType.First:
                        this.document.ActivePageIndex = 0;
                        break;
                    case NavigationRequestType.Previous:
                        this.document.ActivePageIndex--;
                        break;
                    case NavigationRequestType.Next:
                        this.document.ActivePageIndex++;
                        break;
                    case NavigationRequestType.Last:
                        this.document.ActivePageIndex = this.document.Pages.Count - 1;
                        break;
                    case NavigationRequestType.Absolute:
                        int requestedIndex = (int)parameter;

                        if (requestedIndex < 0 || requestedIndex >= this.document.Pages.Count)
                        {
                            break;
                        }

                        this.document.ActivePageIndex = requestedIndex;
                        break;
                    default:
                        throw new InvalidOperationException("Navigation request type is out of range: " + this.navigationRequestType);
                }

                this.mainWindow.AutoScale();

                this.mainWindow.FadeIn(null);
            }));
        }
    }
}
