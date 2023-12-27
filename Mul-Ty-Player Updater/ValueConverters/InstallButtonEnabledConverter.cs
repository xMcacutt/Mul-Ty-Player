using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Mul_Ty_Player_Updater;

/// <summary>
///     A converter that takes in a string and bool to decide if to hide it or not/>
/// </summary>
public class InstallButtonEnabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var installClient = (bool)values[0];
        var clientPath = (string?)values[1];
        var installServer = (bool)values[2];
        var serverPath = (string?)values[3];
        var installGame = (bool)values[4];
        var tyPath = (string?)values[5];
        var mtpPath = (string?)values[6];
        var installing = (bool)values[7];

        if (installing) return false;
        if (!installClient && !installServer && !installGame) return false;
        var result = true;
        if (installClient)
            result = result && Directory.Exists(clientPath);
        if (installServer)
            result = result && Directory.Exists(serverPath);
        if (installGame)
            result = result && CheckGamePaths(tyPath, mtpPath);
        return result;
    }

    private bool CheckGamePaths(string? tyPath, string? mtpPath)
    {
        if (tyPath == null || mtpPath == null) return false;
        return TyData.TyFileNames.All(fileName => File.Exists(Path.Combine(tyPath, fileName)))
            && Directory.Exists(mtpPath);
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
