using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class SelectColorCommand : ButtonPopupCommand
    {
        public SelectColorCommand(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
        }

        protected override Func<bool> CanExecuteFunction => new Func<bool>(() => { return true; });

        protected override Func<MainWindowViewModel, Window, UserControl> ViewFunction => new Func<MainWindowViewModel, Window, UserControl>((MainWindowViewModel mainWindowViewModel, Window window) =>
        {
            return new Views.SelectColorView() { DataContext = new SelectColorViewModel(mainWindowViewModel.InkCanvas, new List<Color> { Colors.LightGray, Colors.Blue, Colors.Green, Colors.Red }, window) };
        });
    }
}
