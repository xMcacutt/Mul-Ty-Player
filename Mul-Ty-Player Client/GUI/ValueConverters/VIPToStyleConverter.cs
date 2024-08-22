using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MulTyPlayerClient.Classes.Utility;

namespace MulTyPlayerClient.GUI;

public class VIPToStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is VIP vip)
        {
            object style = vip switch
            {
                VIP.Matt => Application.Current.Resources["MattStyle"],
                VIP.Buzchy => Application.Current.Resources["BuzchyStyle"],
                VIP.Sirbeyy => Application.Current.Resources["SirbeyyStyle"],
                VIP.Kythol => Application.Current.Resources["KytholStyle"],
                _ => Application.Current.Resources["DefaultStyle"]
            };

            if (style == null)
            {
                // Log or handle the case where style is null
                // Example: Debug.WriteLine($"Resource not found for VIP value: {vip}");
                style = Application.Current.Resources["DefaultStyle"];
            }

            return style;
        }
        return Application.Current.Resources["DefaultStyle"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}