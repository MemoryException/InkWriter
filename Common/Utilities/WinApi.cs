using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Utilities
{
    public static class WinApi
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref ShellFileInfo psfi, uint cbSizeFileInfo, uint uFlags);

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private const uint FILE_READ_EA = 0x0008;
        private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetFinalPathNameByHandle(IntPtr hFile, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
                [MarshalAs(UnmanagedType.LPTStr)] string filename,
                [MarshalAs(UnmanagedType.U4)] uint access,
                [MarshalAs(UnmanagedType.U4)] FileShare share,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                [MarshalAs(UnmanagedType.U4)] uint flagsAndAttributes,
                IntPtr templateFile);

        public static string GetReparsePointFileName(string path)
        {
            IntPtr fileHandle = CreateFile(
                path,
                FILE_READ_EA,
                FileShare.ReadWrite | FileShare.Delete,
                IntPtr.Zero,
                FileMode.Open,
                FILE_FLAG_BACKUP_SEMANTICS,
                IntPtr.Zero);

            if (fileHandle == INVALID_HANDLE_VALUE)
            {
                throw new Win32Exception();
            }

            try
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                var res = GetFinalPathNameByHandle(fileHandle, stringBuilder, 1024, 0);
                if (res == 0)
                {
                    throw new Win32Exception();
                }

                return stringBuilder.ToString().Substring(4);
            }
            finally
            {
                CloseHandle(fileHandle);
            }
        }
    }
}
