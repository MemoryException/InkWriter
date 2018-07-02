using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wpf.Dialogs.Commands;

namespace Wpf.Dialogs
{
    internal class FileDialogViewModel : BaseViewModel
    {
        private string title;
        private Place selectedPlace;
        private FileDialogItem selectedItem;
        private string currentPath;
        private SearchPattern selectedSearchPattern;

        public FileDialogViewModel(IEnumerable<SearchPattern> searchPatterns, SearchPattern selectedSearchPattern)
        {
            this.Places = new Places();
            this.Items = new FileDialogItems();
            this.SearchPatterns = new ObservableCollection<SearchPattern>(searchPatterns);
            this.SelectedSearchPattern = selectedSearchPattern;
            this.Result = FileDialogResultEnum.Unknown;
            this.InitializeCommands();
        }

        private void InitializeCommands()
        {
            this.OkCommand = new OkCommand(this);
            this.CancelCommand = new CancelCommand(this);
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value != this.title)

                {
                    this.title = value;
                    this.InvokePropertyChanged(() => this.Title);
                }
            }
        }

        public Places Places { get; private set; }

        public FileDialogItems Items { get; private set; }

        public OkCommand OkCommand { get; private set; }

        public CancelCommand CancelCommand { get; private set; }

        public FileDialogBehavior Behavior { get; set; }

        public string CurrentPath
        {
            get
            {
                return this.currentPath;
            }
            set
            {
                if (value != this.currentPath)
                {
                    this.currentPath = value;
                    this.InvokePropertyChanged(() => this.CurrentPath);

                    this.ReloadItems();
                    this.OkCommand.RecheckExecute();

                    this.CurrentFile = string.Empty;
                }
            }
        }

        public FileDialogItem SelectedItem
        {
            get
            {
                return this.selectedItem;
            }
            set
            {
                if (value != this.selectedItem)
                {
                    this.selectedItem = value;
                    this.InvokePropertyChanged(() => this.SelectedItem);

                    // Postback from UI, selected item disappears on change directory. Do not update path in this case.
                    if (value == null)
                    {
                        return;
                    }

                    switch (value.ItemType)
                    {
                        case FileDialogItemType.Directory:
                            this.CurrentPath = value.Path;
                            this.CurrentFile = string.Empty;
                            this.OkCommand.RecheckExecute();
                            break;
                        case FileDialogItemType.File:
                            this.CurrentFile = value.Path;
                            this.OkCommand.RecheckExecute();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("value.ItemType");
                    }
                }
            }
        }

        public Place SelectedPlace
        {
            get
            {
                return this.selectedPlace;
            }
            set
            {
                if (value != this.selectedPlace)
                {
                    this.selectedPlace = value;
                    this.InvokePropertyChanged(() => this.SelectedPlace);
                }

                if (value == null)
                {
                    this.CurrentPath = string.Empty;
                    return;
                }

                this.CurrentPath = value.Path;
            }
        }

        public SearchPattern SelectedSearchPattern
        {
            get
            {
                return this.selectedSearchPattern;
            }
            set
            {
                if (value != this.selectedSearchPattern)
                {
                    this.selectedSearchPattern = value;
                    this.InvokePropertyChanged(() => this.SelectedSearchPattern);

                    this.ReloadItems();
                }
            }
        }

        public ObservableCollection<SearchPattern> SearchPatterns { get; private set; }
        public FileDialogResultEnum Result { get; internal set; }
        public string CurrentFile { get; private set; }

        private void ReloadItems()
        {
            this.Items.Reload(this.CurrentPath, this.SelectedSearchPattern.Pattern, this.Behavior);
        }
    }
}
