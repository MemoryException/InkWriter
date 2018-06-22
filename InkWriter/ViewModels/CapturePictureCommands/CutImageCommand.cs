using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.CapturePictureCommands
{
    public class CutImageCommand : ICommand
    {
        private CapturePictureViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public CutImageCommand(CapturePictureViewModel viewModel)
        {
            Safeguard.EnsureNotNull("viewModel", viewModel);

            this.viewModel = viewModel;
            /// TODO: Must get IDisopsable
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
            return this.viewModel.State == CapturePictureViewState.StoppedCapture;
        }

        public void Execute(object parameter)
        {
            this.viewModel.CutImage();
        }
    }
}
