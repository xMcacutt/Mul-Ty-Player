using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class RoleToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        if (parameter == null)
            return (HSRole)value == HSRole.Hider ? "\uf54b" : "\uf002";
        return (HSRole)value == HSRole.Seeker ? "\uf54b" : "\uf002";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}