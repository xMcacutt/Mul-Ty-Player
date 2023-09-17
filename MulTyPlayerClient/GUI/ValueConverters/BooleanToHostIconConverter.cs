using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MulTyPlayerClient.GUI
{
    public class BooleanToHostIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isHost && isHost)
            {
                // Return an icon that represents the host
                Uri uri = new(@"pack://siteoforigin:,,,/GUI/Icons/Icons.xaml");
                ResourceDictionary resourceDict = new ResourceDictionary() { Source = uri };
                DrawingGroup verifiedDrawingGroup = resourceDict["verified_black_24dpDrawingGroup"] as DrawingGroup;
                if (verifiedDrawingGroup != null)
                {
                    return new DrawingImage(verifiedDrawingGroup);
                }
                else return null;
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
