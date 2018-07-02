using System.Collections.Generic;
using System.Windows;

namespace Wpf.Dialogs
{
    public class FileDialog
    {
        public static FileDialogResult Show(string title, IEnumerable<SearchPattern> searchPatterns, SearchPattern selectedSearchPattern)
        {
            FileDialogViewModel fileDialogViewModel = new FileDialogViewModel(searchPatterns, selectedSearchPattern)
            {
                Title = title,
                Behavior = FileDialogBehavior.HideSystemFilesAndDirectories | FileDialogBehavior.HideHiddenFilesAndDirectories
            };

            FileDialogView fileDialogView = new FileDialogView
            {
                DataContext = fileDialogViewModel
            };

            Window window = new Window
            {
                Content = fileDialogView,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = Application.Current.MainWindow.ActualHeight / 2,
                Width = Application.Current.MainWindow.ActualWidth / 2,
                Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.ActualWidth / 4,
                Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight / 4,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowStyle = WindowStyle.SingleBorderWindow
            };

            window.ShowDialog();

            return new FileDialogResult(
                fileDialogViewModel.Result,
                fileDialogViewModel.CurrentFile);
        }
    }
}
