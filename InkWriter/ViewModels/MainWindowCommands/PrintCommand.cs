using InkWriter.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class PrintCommand : ICommand
    {
        private InkWriterDocument document;

        public event EventHandler CanExecuteChanged;

        public PrintCommand(InkWriterDocument document)
        {
            ResetDocument(document);

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
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                PrintDialog printDialog = new PrintDialog
                {
                    SelectedPagesEnabled = false,
                    CurrentPageEnabled = true,
                    UserPageRangeEnabled = true
                };

                if (printDialog.ShowDialog().GetValueOrDefault())
                {
                    FixedDocument fixedDocument = new FixedDocument();

                    switch (printDialog.PageRangeSelection)
                    {
                        case PageRangeSelection.CurrentPage:
                            Application.Current.InvokeOnGui(DispatcherPriority.Normal, () =>
                            {
                                fixedDocument.Pages.Add(this.document.ActivePage.PageContent);
                            });
                            break;
                        case PageRangeSelection.AllPages:
                            foreach (Data.Page page in this.document.Pages)
                            {
                                fixedDocument.Pages.Add(page.PageContent);
                            }
                            break;
                        case PageRangeSelection.UserPages:
                            Application.Current.InvokeOnGui(DispatcherPriority.Normal, () =>
                            {
                                for (int i = printDialog.PageRange.PageFrom - 1; i < printDialog.PageRange.PageTo; ++i)
                                {
                                    fixedDocument.Pages.Add(this.document.Pages[i].PageContent);
                                }
                            });
                            break;
                    }

                    printDialog.PrintDocument(fixedDocument.DocumentPaginator, this.document.FilePath ?? "InkWriter unnamed document");
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
