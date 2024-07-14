using System;
using System.Globalization;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI;

public class ClientIdToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var id = (ushort)value;
        return !PlayerHandler.TryGetPlayer(id, out var player) ? "Unknown Player" : player.Name;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}