using System;
using System.Collections.ObjectModel;
using System.IO;
using Utilities;

namespace Wpf.Dialogs
{
    public class FileDialogItems : ObservableCollection<FileDialogItem>
    {
        internal void Reload(string path, string searchPattern, FileDialogBehavior behavior)
        {
            this.Items.Clear();

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string solvedPath = path;

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                solvedPath = WinApi.GetReparsePointFileName(path);
            }

            DirectoryInfo parentDirectory = Directory.GetParent(path);
            if (parentDirectory != null)
            {
                this.Items.Add(new FileDialogItem(FileDialogItemType.Directory, "..", parentDirectory.FullName));
            }

            try
            {
                ReloadDirectories(solvedPath, behavior);
                ReloadFiles(solvedPath, searchPattern, behavior);
            }
            catch (Exception ex)
            {
                this.Items.Add(new FileDialogItem(FileDialogItemType.Directory, ex.Message, string.Empty));
            }

            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        private void ReloadFiles(string path, string searchPattern, FileDialogBehavior behavior)
        {
            string[] files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
            Array.Sort(files, StringComparer.InvariantCulture);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (behavior.HasFlag(FileDialogBehavior.HideHiddenFilesAndDirectories)
                    && fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    continue;
                }

                if (behavior.HasFlag(FileDialogBehavior.HideSystemFilesAndDirectories)
                    && fileInfo.Attributes.HasFlag(FileAttributes.SparseFile))
                {
                    continue;
                }

                this.Items.Add(new FileDialogItem(FileDialogItemType.File, Path.GetFileName(file), file));
            }
        }

        private void ReloadDirectories(string path, FileDialogBehavior behavior)
        {
            string[] directories = Directory.GetDirectories(path);
            Array.Sort(directories, StringComparer.InvariantCulture);
            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                if (behavior.HasFlag(FileDialogBehavior.HideHiddenFilesAndDirectories)
                    && directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    if (behavior.HasFlag(FileDialogBehavior.HideReparsePoints)
                        || !directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        continue;
                    }
                }

                if (behavior.HasFlag(FileDialogBehavior.HideSystemFilesAndDirectories)
                    && directoryInfo.Attributes.HasFlag(FileAttributes.SparseFile))
                {
                    continue;
                }

                this.Items.Add(new FileDialogItem(FileDialogItemType.Directory, Path.GetFileName(directory), directory));
            }
        }
    }
}