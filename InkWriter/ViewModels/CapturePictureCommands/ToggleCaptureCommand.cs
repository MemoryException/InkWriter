using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.CapturePictureCommands
{
    public class ToggleCaptureCommand : ICommand
    {
        private CapturePictureViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public ToggleCaptureCommand(CapturePictureViewModel viewModel)
        {
            Safeguard.EnsureNotNull("viewModel", viewModel);

            this.viewModel = viewModel;
            /// TODO: Must get IDisopsable.
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
            return this.viewModel.State == CapturePictureViewState.LiveCapturing
                || this.viewModel.State == CapturePictureViewState.StoppedCapture
                || this.viewModel.State == CapturePictureViewState.AcquisitionCompleted;
        }

        public void Execute(object parameter)
        {
            switch (viewModel.State)
            {
                case CapturePictureViewState.LiveCapturing:
                    this.viewModel.StopLiveVideo();
                    break;
                case CapturePictureViewState.StoppedCapture:
                case CapturePictureViewState.AcquisitionCompleted:
                    this.viewModel.StartLiveVideo();
                    break;
                default:
                    throw new InvalidOperationException("Cannot stop/start capturing while in state: " + viewModel.State);
            }
        }
    }
}
