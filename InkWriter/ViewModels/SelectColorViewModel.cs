using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels
{
    public class SelectColorViewModel : BaseViewModel
    {
        private Window window;

        private InkCanvas inkCanvas;

        public SelectColorViewModel(InkCanvas inkCanvas, List<Color> selectableColors, Window window)
        {
            Safeguard.EnsureNotNull("window", window);
            Safeguard.EnsureNotNull("inkCanvas", inkCanvas);
            Safeguard.EnsureNotNull("selectableColors", selectableColors);

            this.window = window;
            this.inkCanvas = inkCanvas;
            this.SelectableColors = selectableColors;

            this.CloseCommand = new CommonCommands.WindowCloseCommand(window);
        }

        public CommonCommands.WindowCloseCommand CloseCommand { get; private set; }

        public List<Color> SelectableColors { get; private set; }

        public Color? SelectedColor
        {
            get
            {
                return this.inkCanvas?.DefaultDrawingAttributes?.Color;
            }

            set
            {
                if (value != null)
                {
                    this.inkCanvas.DefaultDrawingAttributes.Color = (Color)value;
                    this.window.Close();
                }
            }
        }
    }
}
