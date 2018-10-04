using InkWriter.Data;
using InkWriter.ViewModels.MainWindowCommands;
using Microsoft.Ink;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf;

namespace InkWriter.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private InkWriterDocument document;

        private InkWriterSettings settings;

        private static Color SubstitutionColor = Colors.Black;

        private bool dragDropInProgress;
        private Point initialDragPoint;
        private Point polledMouseCoords;
        private StrokeCollection strokesToMove;
        private bool fingerSelectionMode;
        private GridType gridType;

        public void FadeIn(EventHandler completedEvent)
        {
            this.InkCanvas.Fade(0, 1, TimeSpan.FromSeconds(0.15), completedEvent);
        }

        public void FadeOut(EventHandler completedEvent)
        {
            this.InkCanvas.Fade(1, 0, TimeSpan.FromSeconds(0.15), completedEvent);
        }

        public MainWindowViewModel()
        {
            this.settings = InkWriterSettings.Load();

            this.InkCanvas = new InkCanvas();
            this.InkCanvas.PreviewStylusDown += this.OnInkCanvasPreviewStylusDown;
            this.InkCanvas.StylusUp += this.OnInkCanvasStylusUp;
            this.InkCanvas.Background = Brushes.Black;
            this.InkCanvas.DefaultDrawingAttributes.Color = Colors.LightGray;
            this.InkCanvas.DefaultDrawingAttributes.Width = 1;
            this.InkCanvas.DefaultDrawingAttributes.FitToCurve = true;
            this.InkCanvas.PreviewMouseDown += this.OnInkCanvasPreviewMouseDown;
            this.InkCanvas.MouseMove += this.OnInkCanvasMouseMove;
            this.InkCanvas.MouseUp += this.OnInkCanvasMouseUp;
            this.InkCanvas.PreviewMouseUp += this.OnInkCanvasPreviewMouseUp;
            this.InkCanvas.AllowDrop = true;
            this.InkCanvas.Gesture += this.HandleInkCanvasGesture;

            this.InkCanvas.EditingMode = InkCanvasEditingMode.None;

            this.GridType = GridType.None;

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length == 2)
            {
                string filePath = commandLineArgs[1];
                if (File.Exists(filePath))
                {
                    this.document = InkWriterDocument.Load(filePath);
                }
            }

            this.document = new InkWriterDocument();
            this.document.Pages.Add(new Data.Page(this.document));

            this.document.PageChanged += this.OnDocumentPageChanged;
            this.document.ActivePageIndex = this.document.Pages.Count - 1;

            this.NewPageCommand = new NewPageCommand(this, this.document);
            this.FirstPageCommand = new PageNavigationCommand(this, this.document, NavigationRequestType.First);
            this.NextPageCommand = new PageNavigationCommand(this, this.document, NavigationRequestType.Next);
            this.PreviousPageCommand = new PageNavigationCommand(this, this.document, NavigationRequestType.Previous);
            this.LastPageCommand = new PageNavigationCommand(this, this.document, NavigationRequestType.Last);
            this.DeletePageCommand = new DeletePageCommand(this, this.document);
            this.SaveDocumentCommand = new SaveDocumentCommand(this.document);
            this.LoadDocumentCommand = new LoadDocumentCommand(this);
            this.SelectColorCommand = new SelectColorCommand(this);
            this.SelectWidthCommand = new SelectWidthCommand(this);
            this.CapturePictureCommand = new CapturePictureCommand(this);
            this.ToggleGridCommand = new ToggleGridCommand(this);

            this.CopyToClipboardCommand = new CopyToClipboardCommand(() =>
            {
                return this.document?.ActivePage?.GetBitmap(null);
            });

            this.CloseApplicationCommand = new CloseApplicationCommand(this, this.document, this.settings);
            this.MinimizeWindowCommand = new MinimizeWindowCommand(Application.Current.MainWindow);
            this.PrintCommand = new PrintCommand(this.document);
        }

        private void HandleInkCanvasGesture(object sender, InkCanvasGestureEventArgs e)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<GestureRecognitionResult> gestureResults = e.GetGestureRecognitionResults();

            System.Windows.Ink.RecognitionConfidence? lowestConfidence = null;
            System.Windows.Ink.ApplicationGesture? applicationGesture = null;

            foreach (GestureRecognitionResult gestureResult in gestureResults)
            {
                if (lowestConfidence == null || gestureResult.RecognitionConfidence < lowestConfidence)
                {
                    lowestConfidence = gestureResult.RecognitionConfidence;
                    applicationGesture = gestureResult.ApplicationGesture;
                }
            }

            switch (applicationGesture)
            {
                case System.Windows.Ink.ApplicationGesture.Right:
                    if (this.PreviousPageCommand.CanExecute(null))
                    {
                        this.PreviousPageCommand.Execute(null);
                    }

                    break;
                case System.Windows.Ink.ApplicationGesture.Left:
                    if (this.NextPageCommand.CanExecute(null))
                    {
                        this.NextPageCommand.Execute(null);
                    }

                    break;
                case System.Windows.Ink.ApplicationGesture.Check:
                    this.AutoScale();
                    break;

                default:
                    break;
            }
        }

        private void OnInkCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                return;
            }
        }

        public void SetActiveDocument(InkWriterDocument documentToSet)
        {
            if (this.document != null)
            {
                this.document.PageChanged -= this.OnDocumentPageChanged;
            }

            this.DefaultDrawingColor = Colors.LightGray;
            this.document = documentToSet;

            this.NewPageCommand.ResetDocument(this.document);
            this.FirstPageCommand.ResetDocument(this.document);
            this.NextPageCommand.ResetDocument(this.document);
            this.PreviousPageCommand.ResetDocument(this.document);
            this.LastPageCommand.ResetDocument(this.document);
            this.DeletePageCommand.ResetDocument(this.document);
            this.SaveDocumentCommand.ResetDocument(this.document);
            this.CloseApplicationCommand.ResetDocument(this.document);
            this.PrintCommand.ResetDocument(this.document);

            this.document.PageChanged += this.OnDocumentPageChanged;
            this.document.ActivePageIndex = 0;
        }

        public Color DefaultDrawingColor
        {
            get
            {
                return this.InkCanvas.DefaultDrawingAttributes.Color;
            }

            set
            {
                this.InkCanvas.DefaultDrawingAttributes.Color = value;
            }
        }

        public int CurrentPageNumber
        {
            get
            {
                return this.document.ActivePageIndex + 1;
            }

            set
            {
                if (value > 0 && value <= this.document.Pages.Count)
                {
                    this.document.ActivePageIndex = value - 1;
                }
            }
        }

        public int PageCount
        {
            get
            {
                return this.document.Pages.Count;
            }
        }

        public bool FingerSelectionMode
        {
            get
            {
                return this.fingerSelectionMode;
            }

            set
            {
                this.fingerSelectionMode = value;
                this.InvokePropertyChanged(() => this.FingerSelectionMode);
            }
        }

        public NewPageCommand NewPageCommand { get; private set; }

        public PageNavigationCommand FirstPageCommand { get; private set; }

        public PageNavigationCommand PreviousPageCommand { get; private set; }

        public PageNavigationCommand NextPageCommand { get; private set; }

        public PageNavigationCommand LastPageCommand { get; private set; }

        public DeletePageCommand DeletePageCommand { get; private set; }

        public SaveDocumentCommand SaveDocumentCommand { get; private set; }

        public LoadDocumentCommand LoadDocumentCommand { get; private set; }

        public SelectColorCommand SelectColorCommand { get; private set; }

        public SelectWidthCommand SelectWidthCommand { get; private set; }

        public CapturePictureCommand CapturePictureCommand { get; private set; }

        public CopyToClipboardCommand CopyToClipboardCommand { get; private set; }

        public CloseApplicationCommand CloseApplicationCommand { get; private set; }

        public MinimizeWindowCommand MinimizeWindowCommand { get; private set; }

        public PrintCommand PrintCommand { get; private set; }

        public ToggleGridCommand ToggleGridCommand { get; private set; }

        private void OnDocumentPageChanged(object sender, Data.EventHandler.ActivePageChangedEventArgs e)
        {
            this.InkCanvas.Strokes.Clear();
            this.InkCanvas.Children.Clear();
            this.InvokePropertyChanged(() => this.InkCanvas);
            this.InvokePropertyChanged(() => this.CurrentPageNumber);
            this.InvokePropertyChanged(() => this.PageCount);

            if (e.ActivePage == null || e.ActivePage.StrokeData == null)
            {
                return;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(e.ActivePage.StrokeData, 0, e.ActivePage.StrokeData.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                StrokeCollection collection = new StrokeCollection(memoryStream);
                this.InkCanvas.Strokes = collection;

                foreach (System.Windows.Ink.Stroke stroke in this.InkCanvas.Strokes)
                {
                    if (stroke.DrawingAttributes.Color == SubstitutionColor)
                    {
                        stroke.DrawingAttributes.Color = Colors.LightGray;
                    }
                }
            }

            foreach (UIElement uiElement in e.ActivePage.CreateChildren())
            {
                this.InkCanvas.Children.Add(uiElement);
            }
        }

        internal void ResetScale()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.InkCanvas.LayoutTransform = new ScaleTransform(1, 1);

                this.InkCanvas.UpdateLayout();
            }), DispatcherPriority.SystemIdle, null);
        }

        internal void AutoScale()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                double controlHeight = this.InkCanvas.ActualHeight;
                double controlWidth = this.InkCanvas.ActualWidth;

                if (this.InkCanvas.Strokes != null)
                {
                    double rightBorder = 0;
                    double bottomBorder = 0;

                    foreach (var stroke in this.InkCanvas.Strokes)
                    {
                        var bounds = stroke.GetBounds();

                        if (bounds.Right > rightBorder)
                        {
                            rightBorder = bounds.Right;
                        }

                        if (bounds.Bottom > bottomBorder)
                        {
                            bottomBorder = bounds.Bottom;
                        }
                    }

                    double xScale = rightBorder > 0 ? controlWidth / rightBorder : 1;
                    double yScale = bottomBorder > 0 ? controlHeight / bottomBorder : 1;

                    double scale = xScale < yScale ? xScale : yScale;
                    scale = scale < 1 ? scale : 1;
                    this.InkCanvas.LayoutTransform = new ScaleTransform(scale, scale);
                }
                else
                {
                    this.InkCanvas.LayoutTransform = new ScaleTransform(1, 1);
                }

                this.InkCanvas.UpdateLayout();
            }), DispatcherPriority.SystemIdle, null);
        }

        private void OnInkCanvasPreviewStylusDown(object sender, System.Windows.Input.StylusDownEventArgs e)
        {
            if (e.StylusDevice.TabletDevice.Type != TabletDeviceType.Stylus)
            {
                return;
            }

            this.InkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void OnInkCanvasStylusUp(object sender, StylusEventArgs e)
        {
            // Finger in finger selection mode
            if (this.fingerSelectionMode && e.StylusDevice != null && e.StylusDevice.StylusButtons.Count == 1)
            {
                return;
            }

            this.InkCanvas.EditingMode = InkCanvasEditingMode.None;

            this.UpdatePage();
        }

        internal void UpdatePage()
        {
            using (InkCollector collector = new InkCollector())
            using (Stream stream = new MemoryStream())
            {
                StrokeCollection collection = this.InkCanvas.Strokes;
                collection.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                this.document.ActivePage.SetStrokeData(buffer);
                this.document.ActivePage.SetChildren(this.InkCanvas.Children);
            }
        }

        public InkCanvas InkCanvas { get; private set; }

        public GridType GridType
        {
            get
            {
                return this.gridType;
            }
            set
            {
                if (value != this.gridType)
                {
                    this.gridType = value;
                }

                this.InvokePropertyChanged(() => this.GridType);
            }
        }

        public void OnInkCanvasPreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            // Finger
            if (e.StylusDevice != null && e.StylusDevice.StylusButtons.Count == 1 && !this.fingerSelectionMode)
            {
                this.InkCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;
                return;
            }

            // Stylus
            if (e.StylusDevice != null && e.StylusDevice.StylusButtons.Count == 2)
            {
                return;
            }

            this.InkCanvas.EditingMode = InkCanvasEditingMode.Select;

            InkCanvas ic = (InkCanvas)sender;

            Point pt = e.GetPosition(ic);

            if (ic.HitTestSelection(pt) == InkCanvasSelectionHitResult.Selection)
            {
                this.strokesToMove = ic.GetSelectedStrokes();

                this.dragDropInProgress = true;
                this.initialDragPoint = pt;
                this.polledMouseCoords = new Point(pt.X, pt.Y);
            }
        }

        private void OnInkCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragDropInProgress && this.strokesToMove != null)
            {
                Point currentPosition = e.GetPosition((InkCanvas)sender);
                Point delta = new Point(currentPosition.X - this.polledMouseCoords.X, currentPosition.Y - this.polledMouseCoords.Y);
                polledMouseCoords = currentPosition;

                TranslateStrokes(this.strokesToMove, delta.X, delta.Y);
            }
        }

        private void OnInkCanvasPreviewMouseUp(object sender, MouseEventArgs e)
        {
            this.dragDropInProgress = false;
            this.strokesToMove = null;

            this.UpdatePage();
        }

        private static void TranslateStrokes(StrokeCollection strokes, double x, double y)
        {
            Matrix mat = new Matrix();
            mat.Translate(x, y);
            strokes.Transform(mat, false);
        }
    }
}
