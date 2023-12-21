using System;
using System.Globalization;
using System.Windows;

namespace MulTyPlayerClient.GUI;

/// <summary>
///     A converter that takes in a boolean and returns a <see cref="Visibility" />
/// </summary>
public class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter == null)
            return (bool)value ? Visibility.Hidden : Visibility.Visible;
        return (bool)value ? Visibility.Visible : Visibility.Hidden;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}