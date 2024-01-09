using System;
using System.Globalization;
using System.Windows;

namespace MulTyPlayerClient.GUI;

/// <summary>
///     A converter that takes in a boolean and returns a <see cref="Visibility" />
/// </summary>
public class BooleanToVisibilityIconConverter : BaseValueConverter<BooleanToVisibilityIconConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "\uf070" : "\uf06e";
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}