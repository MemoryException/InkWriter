using InkWriter.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
                    UserPageRangeEnabled = true,
                    PageRangeSelection = PageRangeSelection.CurrentPage
                };

                if (printDialog.ShowDialog().GetValueOrDefault())
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    FixedDocument fixedDocument = new FixedDocument();

                    Orientation orientation = printDialog.PrintableAreaHeight > printDialog.PrintableAreaWidth ? Orientation.Vertical : Orientation.Horizontal;

                    switch (printDialog.PageRangeSelection)
                    {
                        case PageRangeSelection.CurrentPage:
                            Application.Current.InvokeOnGui(DispatcherPriority.Normal, () =>
                            {
                                fixedDocument.Pages.Add(this.GetPageContent(printDialog, this.document.ActivePage.GetBitmap(orientation)));
                            });
                            break;
                        case PageRangeSelection.AllPages:
                            Application.Current.InvokeOnGui(DispatcherPriority.Normal, () =>
                            {
                                foreach (Data.Page page in this.document.Pages)
                                {
                                    fixedDocument.Pages.Add(this.GetPageContent(printDialog, page.GetBitmap(orientation)));
                                }
                            });
                            break;
                        case PageRangeSelection.UserPages:
                            Application.Current.InvokeOnGui(DispatcherPriority.Normal, () =>
                            {
                                for (int i = printDialog.PageRange.PageFrom - 1; i < printDialog.PageRange.PageTo; ++i)
                                {
                                    fixedDocument.Pages.Add(this.GetPageContent(printDialog, this.document.Pages[i].GetBitmap(orientation)));
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

        private PageContent GetPageContent(PrintDialog printDialog, BitmapSource bitmapSource)
        {
            Safeguard.EnsureNotNull("printDialog", printDialog);

            PageContent pageContent = new PageContent
            {
                Width = printDialog.PrintableAreaWidth,
                Height = printDialog.PrintableAreaHeight,

                Child = new FixedPage()
                {
                    Width = printDialog.PrintableAreaWidth,
                    Height = printDialog.PrintableAreaHeight,
                }
            };

            Image pageImage = new Image();
            pageImage.BeginInit();
            FixedPage.SetLeft(pageImage, pageContent.Width * 0.05);
            FixedPage.SetTop(pageImage, pageContent.Height * 0.05);
            pageImage.Stretch = System.Windows.Media.Stretch.Uniform;
            pageImage.Width = pageContent.Width * 0.9;
            pageImage.Height = pageContent.Height * 0.9;
            pageImage.HorizontalAlignment = HorizontalAlignment.Center;
            pageImage.VerticalAlignment = VerticalAlignment.Center;
            pageImage.Source = bitmapSource;
            pageImage.EndInit();

            pageImage.Measure(pageContent.RenderSize);
            pageImage.UpdateLayout();

            pageContent.Child.Children.Add(pageImage);

            return pageContent;
        }
    }
}
