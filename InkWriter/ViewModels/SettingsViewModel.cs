using System.Windows;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(Window window)
        {
            Safeguard.EnsureNotNull("window", window);

            this.CloseCommand = new CommonCommands.WindowCloseCommand(window);
        }

        public CommonCommands.WindowCloseCommand CloseCommand { get; private set; }
    }
}
