using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Utilities;

namespace Wpf.Dialogs
{
    public class FileDialogItem
    {
        public FileDialogItemType ItemType { get; private set; }
        public string Text { get; private set; }
        public string Path { get; private set; }
        public BitmapSource Icon
        {
            get
            {
                ShellFileInfo shellFileInfo = new ShellFileInfo();
                WinApi.SHGetFileInfo(this.Path, 0, ref shellFileInfo, (uint)Marshal.SizeOf(shellFileInfo), WinApi.SHGFI_ICON | WinApi.SHGFI_SMALLICON);
                Bitmap bitmap = System.Drawing.Icon.FromHandle(shellFileInfo.handleIcon).ToBitmap();
                IntPtr bitmapHandle = bitmap.GetHbitmap();
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapHandle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public FileDialogItem(FileDialogItemType itemType, string text, string path)
        {
            this.ItemType = itemType;
            this.Text = text;
            this.Path = path;
        }
    }
}