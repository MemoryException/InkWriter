using InkWriter.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace InkWriter.Converters
{
    [ValueConversion(typeof(CapturePictureViewState), typeof(string))]
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CapturePictureViewState? state = value as CapturePictureViewState?;
            if (state == null)
            {
                return null;
            }

            switch (state)
            {
                case CapturePictureViewState.AcquiringImage:
                case CapturePictureViewState.LiveCapturing:
                    return "Pause";
                case CapturePictureViewState.StoppedCapture:
                    return "Rec";
                default:
                    return "Error";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
