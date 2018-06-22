using System.Windows;
using System.Windows.Input;

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
        }
    }
}
