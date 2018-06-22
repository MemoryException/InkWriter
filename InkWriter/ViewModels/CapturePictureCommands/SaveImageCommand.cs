using Microsoft.Win32;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.CapturePictureCommands
{
    public class SaveImageCommand : ICommand
    {
        private CapturePictureViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public SaveImageCommand(CapturePictureViewModel viewModel)
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Image...";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = DateTime.Now.ToString("yyyy-MM-dd hhmmss") + ".jpg";
            saveFileDialog.DefaultExt = "jpg";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "JPEG-Images (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                this.viewModel.PropertyChanged -= this.OnViewModelPropertyChanged;
                this.viewModel.SaveImage(saveFileDialog.FileName);
                this.viewModel.CloseCommand.Execute(null);
            }
        }
    }
}
