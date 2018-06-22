using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class CopyToClipboardCommand : ICommand
    {
        private Func<BitmapSource> functionToRetrieveImage;

        public CopyToClipboardCommand(Func<BitmapSource> functionToRetrieveImage)
        {
            Safeguard.EnsureNotNull("functionToRetrieveText", functionToRetrieveImage);

            this.functionToRetrieveImage = functionToRetrieveImage;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                BitmapSource imageToCopy = this.functionToRetrieveImage.Invoke();
                Clipboard.Clear();
                Clipboard.SetImage(imageToCopy);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, Properties.Resources.CopyToClipboardFailed, ex.Message), Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
