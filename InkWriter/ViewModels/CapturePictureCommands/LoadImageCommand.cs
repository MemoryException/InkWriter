using Microsoft.Win32;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.CapturePictureCommands
{
    public class LoadImageCommand : ICommand
    {
        private CapturePictureViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public LoadImageCommand(CapturePictureViewModel viewModel)
        {
            Safeguard.EnsureNotNull("viewModel", viewModel);

            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                this.CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.viewModel.State == CapturePictureViewState.AcquisitionCompleted || this.viewModel.State == CapturePictureViewState.StoppedCapture;
        }

        public void Execute(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Image...";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "jpg";
            openFileDialog.AddExtension = true;
            openFileDialog.Filter = "JPEG-Images (*.jpg)|*.jpg";
            if (openFileDialog.ShowDialog() == true)
            {
                this.viewModel.LoadImage(openFileDialog.FileName);
            }
        }
    }
}
