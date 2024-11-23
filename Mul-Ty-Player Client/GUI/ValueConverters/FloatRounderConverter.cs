
using System;
using System.Globalization;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class FloatRounderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float doubleValue && parameter is int intParam)
        {
            return Math.Round(doubleValue, intParam);
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is float doubleValue && parameter is int intParam)
        {
            return Math.Round(doubleValue, intParam);
        }
        return value;
    }
}