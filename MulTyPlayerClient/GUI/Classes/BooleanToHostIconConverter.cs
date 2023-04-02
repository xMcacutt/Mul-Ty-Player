using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MulTyPlayerClient.GUI
{
    public class BooleanToHostIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isHost && isHost)
            {
                // Return an icon that represents the host
                return Application.Current.Resources["Verified_black_24dpDrawingImage"];
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
