using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class SelectWidthCommand : ButtonPopupCommand
    {
        public SelectWidthCommand(MainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
        }

        protected override Func<bool> CanExecuteFunction => new Func<bool>(() => { return true; });

        protected override Func<MainWindowViewModel, Window, UserControl> ViewFunction => new Func<MainWindowViewModel, Window, UserControl>((MainWindowViewModel mainWindowViewModel, Window window) =>
        {
            return new Views.SelectWidthView() { DataContext = new SelectWidthViewModel(mainWindowViewModel.InkCanvas, new List<double> { 1, 2, 3, 5, 10, 15, 25, 50, 100 }, window) };
        });

    }
}
