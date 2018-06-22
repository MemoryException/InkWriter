using System;
using System.Runtime.InteropServices;

namespace Wpf
{
    public static class DpiUtilities
    {
        private static Lazy<Tuple<float, float>> _dpi = new Lazy<Tuple<float, float>>(ReadDpi);

        public static float DesktopDpiX
        {
            get
            {
                return _dpi.Value.Item1;
            }
        }

        public static float DesktopDpiY
        {
            get
            {
                return _dpi.Value.Item2;
            }
        }

        public static void Reload()
        {
            ID2D1Factory factory;
            int hr = D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(ID2D1Factory).GUID, IntPtr.Zero, out factory);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);

            factory.ReloadSystemMetrics();
            Marshal.ReleaseComObject(factory);
        }

        private static Tuple<float, float> ReadDpi()
        {
            ID2D1Factory factory;
            int hr = D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(ID2D1Factory).GUID, IntPtr.Zero, out factory);
            if (hr != 0)
                Marshal.ThrowExceptionForHR(hr);

            float x;
            float y;
            factory.GetDesktopDpi(out x, out y);
            Marshal.ReleaseComObject(factory);
            return new Tuple<float, float>(x, y);
        }

        [DllImport("d2d1.dll")]
        private static extern int D2D1CreateFactory(D2D1_FACTORY_TYPE factoryType, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, IntPtr pFactoryOptions, out ID2D1Factory ppIFactory);

        private enum D2D1_FACTORY_TYPE
        {
            D2D1_FACTORY_TYPE_SINGLE_THREADED = 0,
            D2D1_FACTORY_TYPE_MULTI_THREADED = 1,
        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("06152247-6f50-465a-9245-118bfd3b6007")]
        private interface ID2D1Factory
        {
            int ReloadSystemMetrics();

            [PreserveSig]
            void GetDesktopDpi(out float dpiX, out float dpiY);
        }
    }
}
