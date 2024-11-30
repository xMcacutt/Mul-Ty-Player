using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class BooleanToActivityIndicatorColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? App.AppColors.MainAccent : App.AppColors.MainText;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}