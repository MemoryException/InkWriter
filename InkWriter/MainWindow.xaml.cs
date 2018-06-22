using System.Windows;

namespace InkWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new ViewModels.MainWindowViewModel();

            InitializeComponent();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.Topmost = false;
                this.WindowStyle = WindowStyle.ToolWindow;
                this.WindowState = WindowState.Normal;
            }
        }
    }
}
