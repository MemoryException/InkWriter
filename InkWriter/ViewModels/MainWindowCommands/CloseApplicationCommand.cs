using InkWriter.Data;
using System;
using System.Windows;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.MainWindowCommands
{
    public class CloseApplicationCommand : ICommand
    {
        private InkWriterDocument document;

        private InkWriterSettings settings;

        private MainWindowViewModel mainWindow;

        public event EventHandler CanExecuteChanged;

        public CloseApplicationCommand(MainWindowViewModel mainWindow, InkWriterDocument document, InkWriterSettings settings)
        {
            Safeguard.EnsureNotNull("mainWindow", mainWindow);
            Safeguard.EnsureNotNull("document", document);
            Safeguard.EnsureNotNull("settings", settings);

            this.document = document;
            this.settings = settings;
            this.mainWindow = mainWindow;

            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public void ResetDocument(InkWriterDocument documentToSet)
        {
            Safeguard.EnsureNotNull("document", documentToSet);

            this.document = documentToSet;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.settings.Save();

            if (!this.document.IsDirty)
            {
                Application.Current.Shutdown();
                return;
            }

            switch (MessageBox.Show(Properties.Resources.SaveChangesQuestion, Properties.Resources.Shutdown, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel))
            {
                case MessageBoxResult.Yes:
                    if (!string.IsNullOrEmpty(this.document.FilePath))
                    {
                        this.document.Save(this.document.FilePath);
                    }
                    else
                    {
                        this.mainWindow.SaveDocumentCommand.Execute(null);
                    }
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.Cancel:
                    return;
            }
        }
    }
}
