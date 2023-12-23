using System;
using System.Globalization;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class PopupHeightToArrowDirectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((double)value == 0)
        {
            return "\uf0d7";
        }
        return "\uf0d8";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}