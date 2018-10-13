using System;
using System.Windows.Input;
using Utilities;
using Wpf.Dialogs;

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
            SearchPattern[] searchPatterns = new SearchPattern[]
            {
                new SearchPattern("Jpegs (*.jpg)", "*.jpg"),
                new SearchPattern("All files (*.*)", "*.*")
            };

            FileDialogResult result = FileDialog.Show("Open File...", searchPatterns, searchPatterns[0]);

            if (result.Result == FileDialogResultEnum.OK)
            {
                this.viewModel.LoadImage(result.SelectedFile);
            }
        }
    }
}
