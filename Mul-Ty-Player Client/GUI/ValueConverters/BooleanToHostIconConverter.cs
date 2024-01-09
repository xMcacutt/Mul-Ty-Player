using System;
using System.Globalization;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class BooleanToHostIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool and true)
            // Return an icon that represents the host
            return "\uf521";
        // Return null if the player is not the host
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}