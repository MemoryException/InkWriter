using InkWriter.Algorithms;
using InkWriter.Converters;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;

namespace InkWriter.ViewModels.CapturePictureCommands
{
    public class EmbeddPictureCommand : ICommand
    {
        private static BitmapToBitmapSource BitmapToBitmapSourceConverter = new BitmapToBitmapSource();

        private CapturePictureViewModel capturePictureView;

        private MainWindowViewModel mainWindowViewModel;

        public EmbeddPictureCommand(CapturePictureViewModel capturePictureView, MainWindowViewModel mainWindow)
        {
            Safeguard.EnsureNotNull("capturePictureView", capturePictureView);
            Safeguard.EnsureNotNull("mainWindow", mainWindow);

            this.capturePictureView = capturePictureView;
            this.mainWindowViewModel = mainWindow;

            this.capturePictureView.PropertyChanged += this.OnViewModelPropertyChanged;
        }

        public event EventHandler CanExecuteChanged;

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                this.CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.capturePictureView.Image != null && (this.capturePictureView.State == CapturePictureViewState.AcquisitionCompleted || this.capturePictureView.State == CapturePictureViewState.StoppedCapture);
        }

        public void Execute(object parameter)
        {
            Safeguard.EnsureNotNull("mainWindowViewModel.InkCanvas", mainWindowViewModel.InkCanvas);

            Bitmap bitmap = (Bitmap)BitmapToBitmapSourceConverter.ConvertBack(this.capturePictureView.Image, typeof(Bitmap), null, CultureInfo.CurrentUICulture);
            Bitmap copy = ImageManipulation.Copy(bitmap); 
            ImageSource imageSource = (ImageSource)BitmapToBitmapSourceConverter.Convert(copy, typeof(ImageSource), null, CultureInfo.CurrentUICulture);

            this.mainWindowViewModel.InkCanvas.Children.Add(new System.Windows.Controls.Image
            {
                Source = imageSource
            });

            this.capturePictureView.PropertyChanged -= this.OnViewModelPropertyChanged;
            this.capturePictureView.CloseCommand.Execute(null);

            this.mainWindowViewModel.UpdatePage();
        }
    }
}
