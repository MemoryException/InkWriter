using AForge.Video.DirectShow;
using InkWriter.Algorithms;
using InkWriter.Converters;
using InkWriter.ViewModels.CapturePictureCommands;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities;
using Wpf;

namespace InkWriter.ViewModels
{
    public enum CapturePictureViewState
    {
        LiveCapturing,
        StoppedCapture,
        AcquiringImage,
        AcquisitionCompleted
    }

    public class CapturePictureViewModel : BaseViewModel
    {
        private System.Windows.Point upperLeftPoint;
        private System.Windows.Point upperRightPoint;
        private System.Windows.Point bottomLeftPoint;
        private System.Windows.Point bottomRightPoint;

        private VideoCaptureDevice videoSource;

        private static BitmapToBitmapSource BitmapToBitmapSourceConverter = new BitmapToBitmapSource();
        private CapturePictureViewState state;
        private bool showGrid;

        public CapturePictureViewState State
        {
            get
            {
                return this.state;
            }

            private set
            {
                this.state = value;
                this.InvokePropertyChanged(() => this.State);
                this.InvokePropertyChanged(() => this.ImageStretchMode);
            }
        }

        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }

        public System.Windows.Point UpperLeftPoint
        {
            get { return this.upperLeftPoint; }
            set
            {
                this.upperLeftPoint = value;
                this.InvokePropertyChanged(() => this.UpperLeftPoint);
            }
        }

        internal void LoadImage(string fileName)
        {
            if (this.State == CapturePictureViewState.LiveCapturing)
            {
                this.StopLiveVideo();
            }

            this.State = CapturePictureViewState.StoppedCapture;

            BitmapImage src = new BitmapImage();
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.BeginInit();
            src.UriSource = new Uri(fileName);
            src.EndInit();

            this.Image = src; // (new ImageSourceConverter()).ConvertFromString(fileName) as ImageSource;
            this.InvokePropertyChanged(() => this.Image);
        }

        public System.Windows.Point UpperRightPoint
        {
            get { return this.upperRightPoint; }
            set
            {
                this.upperRightPoint = value;
                this.InvokePropertyChanged(() => this.UpperRightPoint);
            }
        }
        public System.Windows.Point BottomLeftPoint
        {
            get { return this.bottomLeftPoint; }
            set
            {
                this.bottomLeftPoint = value;
                this.InvokePropertyChanged(() => this.BottomLeftPoint);
            }
        }

        public Stretch ImageStretchMode
        {
            get
            {
                switch (this.State)
                {
                    case CapturePictureViewState.AcquiringImage:
                    case CapturePictureViewState.LiveCapturing:
                    case CapturePictureViewState.StoppedCapture:
                        return Stretch.Fill;
                    case CapturePictureViewState.AcquisitionCompleted:
                        return Stretch.Uniform;
                    default:
                        return Stretch.Fill;
                }
            }
        }

        internal void SaveImage(string fileName)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            BitmapFrame bitmapFrame = BitmapFrame.Create((BitmapSource)this.Image);
            encoder.Frames.Add(bitmapFrame);
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        public System.Windows.Point BottomRightPoint
        {
            get { return this.bottomRightPoint; }
            set
            {
                this.bottomRightPoint = value;
                this.InvokePropertyChanged(() => this.BottomRightPoint);
            }
        }

        public bool ShowGrid
        {
            get
            {
                return this.showGrid;
            }

            private set
            {
                this.showGrid = value;
                this.InvokePropertyChanged(() => this.ShowGrid);
            }
        }

        public CapturePictureViewModel(Window window, MainWindowViewModel mainWindow)
        {
            Safeguard.EnsureNotNull("window", window);
            Safeguard.EnsureNotNull("mainWindow", mainWindow);

            this.CloseCommand = new CommonCommands.WindowCloseCommand(window, () => { this.StopLiveVideo(); });
            this.ToggleVideoCommand = new ToggleCaptureCommand(this);
            this.CutImageCommand = new CutImageCommand(this);
            this.SaveImageCommand = new SaveImageCommand(this);
            this.LoadImageCommand = new LoadImageCommand(this);

            this.State = CapturePictureViewState.StoppedCapture;

            this.StartLiveVideo();

            /// TODO: Release on dispose, CapturePictureView must get disposable
            this.DecreaseZoomCommand = new ZoomImageCommand(ZoomType.DecreaseZoom, this.videoSource);
            this.DecreaseZoomCommand.ZoomChanged += ZoomChanged;
            this.IncreaseZoomCommand = new ZoomImageCommand(ZoomType.IncreaseZoom, this.videoSource);
            this.IncreaseZoomCommand.ZoomChanged += ZoomChanged;
            this.ResetZoomCommand = new ZoomImageCommand(ZoomType.ResetZoom, this.videoSource);
            this.ResetZoomCommand.ZoomChanged += ZoomChanged;
            this.EmbeddPictureCommand = new EmbeddPictureCommand(this, mainWindow);
        }

        private void ZoomChanged(object sender, EventArgs e)
        {
            this.DecreaseZoomCommand.Invalidate();
            this.IncreaseZoomCommand.Invalidate();
            this.ResetZoomCommand.Invalidate();
        }

        public void CutImage()
        {
            Bitmap bitmap = (Bitmap)BitmapToBitmapSourceConverter.ConvertBack(this.Image, typeof(Bitmap), null, CultureInfo.CurrentUICulture);

            System.Drawing.Point upperLeftBitmapPoint = new System.Drawing.Point((int)(this.UpperLeftPoint.X / this.ImageWidth * bitmap.Width), (int)(this.UpperLeftPoint.Y / this.ImageHeight * bitmap.Height));
            System.Drawing.Point upperRightBitmapPoint = new System.Drawing.Point((int)(this.UpperRightPoint.X / this.ImageWidth * bitmap.Width), (int)(this.UpperRightPoint.Y / this.ImageHeight * bitmap.Height));
            System.Drawing.Point bottomLeftBitmapPoint = new System.Drawing.Point((int)(this.BottomLeftPoint.X / this.ImageWidth * bitmap.Width), (int)(this.BottomLeftPoint.Y / this.ImageHeight * bitmap.Height));
            System.Drawing.Point bottomRightBitmapPoint = new System.Drawing.Point((int)(this.BottomRightPoint.X / this.ImageWidth * bitmap.Width), (int)(this.BottomRightPoint.Y / this.ImageHeight * bitmap.Height));

            Bitmap distortedBitmap = ImageManipulation.Cut(bitmap, upperLeftBitmapPoint, upperRightBitmapPoint, bottomLeftBitmapPoint, bottomRightBitmapPoint);

            this.Image = (ImageSource)BitmapToBitmapSourceConverter.Convert(distortedBitmap, typeof(ImageSource), null, CultureInfo.CurrentUICulture);
            this.InvokePropertyChanged(() => this.Image);

            this.State = CapturePictureViewState.AcquisitionCompleted;
            this.ShowGrid = false;
        }

        public ImageSource Image { get; private set; }

        public void StartLiveVideo()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            this.videoSource = new VideoCaptureDevice(videoDevices[1].MonikerString);
            this.videoSource.NewFrame += this.OnVideoSourceNewFrame;
            this.videoSource.Start();

            this.State = CapturePictureViewState.LiveCapturing;
            this.ShowGrid = false;
        }

        public void StopLiveVideo()
        {
            if (this.videoSource != null)
            {
                this.videoSource.Stop();
                this.videoSource.NewFrame -= this.OnVideoSourceNewFrame;
                this.videoSource = null;

                this.State = CapturePictureViewState.StoppedCapture;
            }

            if (this.Image != null)
            {
                this.UpperLeftPoint = new System.Windows.Point(this.ImageWidth * 0.1, this.ImageHeight * 0.1);
                this.UpperRightPoint = new System.Windows.Point(this.ImageWidth * 0.9, this.ImageHeight * 0.1);
                this.BottomLeftPoint = new System.Windows.Point(this.ImageWidth * 0.1, this.ImageHeight * 0.9);
                this.BottomRightPoint = new System.Windows.Point(this.ImageWidth * 0.9, this.ImageHeight * 0.9);

                this.ShowGrid = true;
            }
        }

        private void OnVideoSourceNewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            this.Image = (ImageSource)BitmapToBitmapSourceConverter.Convert(eventArgs.Frame, typeof(ImageSource), null, CultureInfo.CurrentUICulture);
            this.InvokePropertyChanged(() => this.Image);
        }

        public CommonCommands.WindowCloseCommand CloseCommand { get; private set; }

        public ToggleCaptureCommand ToggleVideoCommand { get; private set; }

        public CutImageCommand CutImageCommand { get; private set; }

        public SaveImageCommand SaveImageCommand { get; private set; }

        public LoadImageCommand LoadImageCommand { get; private set; }

        public ZoomImageCommand IncreaseZoomCommand { get; private set; }

        public ZoomImageCommand DecreaseZoomCommand { get; private set; }

        public ZoomImageCommand ResetZoomCommand { get; private set; }

        public EmbeddPictureCommand EmbeddPictureCommand { get; private set; }
    }
}
