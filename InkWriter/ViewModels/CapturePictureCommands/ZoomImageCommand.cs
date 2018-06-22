using AForge.Video.DirectShow;
using System;
using System.Windows.Input;
using Utilities;

namespace InkWriter.ViewModels.CapturePictureCommands
{
    public enum ZoomType
    {
        Undefined = 0,
        IncreaseZoom,
        ResetZoom,
        DecreaseZoom
    }

    public class ZoomImageCommand : ICommand
    {
        private VideoCaptureDevice videoCaptureDevice;

        private ZoomType zoomType;

        private static int MinZoom;
        private static int MaxZoom;
        private static int StepSize;
        private static int DefaultValue;
        private static CameraControlFlags ControlFlags;
        private static bool Initialized;

        public event EventHandler CanExecuteChanged;

        public event EventHandler ZoomChanged;

        public ZoomImageCommand(ZoomType zoomType, VideoCaptureDevice videoCaptureDevice)
        {
            Safeguard.EnsureNotNull("videoCaptureDevice", videoCaptureDevice);

            this.zoomType = zoomType;
            this.videoCaptureDevice = videoCaptureDevice;

            if (!Initialized)
            {
                this.videoCaptureDevice.GetCameraPropertyRange(CameraControlProperty.Zoom, out MinZoom, out MaxZoom, out StepSize, out DefaultValue, out ControlFlags);
                Initialized = true;
            }
        }

        private int GetZoomValue()
        {
            int value;
            CameraControlFlags controlFlags;
            this.videoCaptureDevice.GetCameraProperty(CameraControlProperty.Zoom, out value, out controlFlags);
            return value;
        }

        public static void Reset()
        {
            Initialized = false;
        }

        private bool IsZoomChangeable()
        {
            switch (this.zoomType)
            {
                case ZoomType.DecreaseZoom:
                    return this.GetZoomValue() > MinZoom;
                case ZoomType.IncreaseZoom:
                    return this.GetZoomValue() < MaxZoom;
                case ZoomType.ResetZoom:
                    return true;
                default:
                    return false;
            }
        }

        public void Invalidate()
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            return this.videoCaptureDevice.IsRunning && this.IsZoomChangeable();
        }

        public void Execute(object parameter)
        {
            switch (this.zoomType)
            {
                case ZoomType.DecreaseZoom:
                    this.videoCaptureDevice.SetCameraProperty(CameraControlProperty.Zoom, this.GetZoomValue() - StepSize, CameraControlFlags.Manual);
                    this.ZoomChanged?.Invoke(this, new EventArgs());
                    break;
                case ZoomType.IncreaseZoom:
                    this.videoCaptureDevice.SetCameraProperty(CameraControlProperty.Zoom, this.GetZoomValue() + StepSize, CameraControlFlags.Manual);
                    this.ZoomChanged?.Invoke(this, new EventArgs());
                    break;
                case ZoomType.ResetZoom:
                    this.videoCaptureDevice.SetCameraProperty(CameraControlProperty.Zoom, DefaultValue, CameraControlFlags.Manual);
                    this.ZoomChanged?.Invoke(this, new EventArgs());
                    break;
                default:
                    return;
            }
        }
    }
}
