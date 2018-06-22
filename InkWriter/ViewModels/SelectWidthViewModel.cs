using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels
{
    public class SelectWidthViewModel : BaseViewModel
    {
        private Window window;

        private InkCanvas inkCanvas;

        public SelectWidthViewModel(InkCanvas inkCanvas, List<double> selectableWidths, Window window)
        {
            Safeguard.EnsureNotNull("window", window);
            Safeguard.EnsureNotNull("inkCanvas", inkCanvas);
            Safeguard.EnsureNotNull("selectableWidths", selectableWidths);

            this.window = window;
            this.inkCanvas = inkCanvas;
            this.SelectableWidths = selectableWidths;

            this.CloseCommand = new CommonCommands.WindowCloseCommand(window);
        }

        public CommonCommands.WindowCloseCommand CloseCommand { get; private set; }

        public List<double> SelectableWidths { get; private set; }

        public double? SelectedWidth
        {
            get
            {
                return this.inkCanvas?.DefaultDrawingAttributes?.Width;
            }

            set
            {
                if (value != null)
                {
                    this.inkCanvas.DefaultDrawingAttributes.Width = (int)value;
                    this.window.Close();
                }
            }
        }
    }
}

