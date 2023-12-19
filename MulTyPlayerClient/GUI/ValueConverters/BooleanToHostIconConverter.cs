using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MulTyPlayerClient.GUI.Controls;

namespace MulTyPlayerClient.GUI
{
    public class BooleanToHostIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool and true)
            {
                // Return an icon that represents the host
                var ico = new Icon();
                ico.Code = "\uf521";
                return ico;
            }
            else
            {
                // Return null if the player is not the host
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
