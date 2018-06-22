using System;
using System.Globalization;
using System.Windows.Data;

namespace InkWriter.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class AddDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? doubleValue = value as double?;
            if (doubleValue == null)
            {
                return null;
            }

            double doubleParameter;
            if (!double.TryParse((string)parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleParameter))
            {
                return null;
            }

            return doubleValue + doubleParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? doubleValue = value as double?;
            if (doubleValue == null)
            {
                return null;
            }

            double doubleParameter;
            if (!double.TryParse((string)parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleParameter))
            {
                return null;
            }

            return doubleValue - doubleParameter;
        }
    }
}
