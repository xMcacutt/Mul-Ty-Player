using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class KoalaToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Koala koala) 
            return null;
        var koalaName = Enum.GetName(typeof(Koala), koala);
        if (koalaName == null)
            return null;
        // Retrieve KoalaIcons resource dictionary by key
        if (Application.Current.Resources["KoalaIcons"] is not ResourceDictionary koalaIcons) 
            return null;
        if (!koalaIcons.Contains(koalaName)) 
            return null;
        var x = koalaIcons[koalaName];
        return x;
        // Return a default style or null if the koalaName is not found
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}