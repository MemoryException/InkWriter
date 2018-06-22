using System;
using System.Windows;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.CommonCommands
{
    public class WindowCloseCommand : ICommand
    {
        private Window window;
        private Action closeAction;

        public event EventHandler CanExecuteChanged;

        public WindowCloseCommand(Window window)
        {
            Safeguard.EnsureNotNull("selectColorView", window);

            this.window = window;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public WindowCloseCommand(Window window, Action closeAction)
            : this(window)
        {
            Safeguard.EnsureNotNull("closeAction", closeAction);
            this.closeAction = closeAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (this.closeAction != null)
            {
                this.closeAction.Invoke();
            }

            this.window.Close();
        }
    }
}
