using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class KoalaNameToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string koalaName)
            // Retrieve KoalaIcons resource dictionary by key
            if (Application.Current.Resources["KoalaIcons"] is ResourceDictionary koalaIcons)
                if (koalaIcons.Contains(koalaName))
                {
                    var x = koalaIcons[koalaName];
                    return x;
                }

        // Return a default style or null if the koalaName is not found
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}