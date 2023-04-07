using System;
using System.Globalization;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI
{
    public class SubstringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && str.Length >= 14)
            {
                return str[..14] + "...";
            }
            // return some default value or throw an exception
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
