using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InkWriter.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? booleanValue = value as bool?;
            if (booleanValue == null)
            {
                return null;
            }

            switch (booleanValue)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Hidden;
                default:
                    return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
