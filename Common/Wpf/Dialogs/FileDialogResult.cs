namespace Wpf.Dialogs
{
    public class FileDialogResult
    {
        public FileDialogResultEnum Result { get; internal set; }

        public string SelectedFile { get; private set; }

        public FileDialogResult(FileDialogResultEnum result, string selectedFile)
        {
            this.Result = result;
            this.SelectedFile = selectedFile;
        }
    }
}