using System;
using System.Runtime.InteropServices;

namespace Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ShellFileInfo
    {
        public IntPtr handleIcon;
        public int icon;
        public uint attributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string displayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string typeName;
    }
}