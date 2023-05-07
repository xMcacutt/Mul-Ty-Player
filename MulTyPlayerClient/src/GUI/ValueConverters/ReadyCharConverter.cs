using System;
using System.Globalization;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI
{
    public class ReadyCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isReady && isReady)
            {
                return "R!";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}