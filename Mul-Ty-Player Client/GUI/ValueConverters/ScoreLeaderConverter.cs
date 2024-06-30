using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class ScoreLeaderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Player)
            return 10;
        return PlayerHandler.Players.Where(x => x != value).Any(otherPlayer => ((Player)value).Score < otherPlayer.Score) ? 10 : 12;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}