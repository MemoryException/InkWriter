using System;

namespace Wpf.Dialogs
{
    [Flags]
    public enum FileDialogBehavior : short
    {
        None = 0,
        HideSystemFilesAndDirectories = 1,
        HideHiddenFilesAndDirectories = 2,
        HideReparsePoints = 4
    }
}
