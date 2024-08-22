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
            switch (vip)
            {
                case VIP.Buzchy:
                    return Application.Current.Resources["BuzchyStyle"];
                case VIP.Matt:
                    return Application.Current.Resources["MattStyle"];
                case VIP.Sirbeyy:
                    return Application.Current.Resources["SirbeyyStyle"];
                default:
                    return Application.Current.Resources["DefaultStyle"];
            }
        }
        return Application.Current.Resources["DefaultStyle"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}