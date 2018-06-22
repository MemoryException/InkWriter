﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class SelectWidthCommand : ICommand
    {
        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public SelectWidthCommand(MainWindowViewModel mainWindow)
        {
            Safeguard.EnsureNotNull("mainWindow", mainWindow);

            this.mainWindow = mainWindow;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Button button = parameter as Button;
            if (button == null)
            {
                return;
            }

            Window window = new Window();
            window.Opacity = 0;
            Point upperLeftPoint = button.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            window.Left = upperLeftPoint.X;
            window.Top = upperLeftPoint.Y + 50;
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Topmost = true;
            Views.SelectWidthView view = new Views.SelectWidthView() { DataContext = new ViewModels.SelectWidthViewModel(this.mainWindow.InkCanvas, new List<double> { 1, 2, 3, 5, 10, 15, 25, 50, 100 }, window) };
            window.Content = view;
            window.WindowState = WindowState.Normal;
            window.Show();

            window.Fade(0, 1, TimeSpan.FromSeconds(0.15), null);
        }
    }
}
