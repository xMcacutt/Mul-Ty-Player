using System;
using System.Globalization;
using System.Windows.Data;

namespace Mul_Ty_Player_Updater;

public class BooleanToReadyIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isReady && isReady) return "\uf11e";

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}