using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf.Dialogs
{
    [ValueConversion(typeof(long), typeof(string))]
    public class ByteUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long? bytes = value as long?;
            if (bytes == null)
            {
                return null;
            }

            string[] units = { "b", "kb", "mb", "gb", "tb" };
            int unitNo = 0;

            while (bytes > 1024)
            {
                bytes /= 1024;
                unitNo++;
            }

            if (unitNo < units.Length)
            {
                return string.Format(culture, "{0} {1}", bytes, units[unitNo]);
            }

            return string.Format(culture, "{0} {1}", bytes, string.Format(culture, "E{0}", unitNo * 4));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
