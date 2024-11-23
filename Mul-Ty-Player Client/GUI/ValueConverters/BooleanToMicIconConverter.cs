using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class BooleanToMicIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "\uf131" : "\uf130";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}