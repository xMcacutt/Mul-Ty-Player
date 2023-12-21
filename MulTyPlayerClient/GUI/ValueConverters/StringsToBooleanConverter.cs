using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient.GUI;

public class StringsToBooleanConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // Check that both values are not null or empty strings
        var tyPath = values[0] as string;
        var mtpPath = values[1] as string;
        if (!string.IsNullOrEmpty(tyPath) && !string.IsNullOrEmpty(mtpPath))
            // Check that the paths are valid folders
            if (Directory.Exists(tyPath) && Directory.Exists(mtpPath) && CheckTyFolder(tyPath) && CheckMTPPath(mtpPath))
                return true;
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private bool CheckMTPPath(string mtpPath)
    {
        if (Directory.GetFiles(mtpPath).Length == 0) return true;
        if (File.Exists(Path.Combine(mtpPath, "Patch_PC.rkv"))) return true;
        return false;
    }

    private bool CheckTyFolder(string tyPath)
    {
        var i = 0;
        foreach (var fileName in ModelController.Setup.TyFileNames)
        {
            var filePath = Path.Combine(tyPath, fileName);
            var fileInfo = new FileInfo(filePath);
            var index = Array.IndexOf(ModelController.Setup.TyFileNames, fileName);
            if (fileInfo.Exists) i++;
        }

        if (i == ModelController.Setup.TyFileNames.Length) return true;
        return false;
    }
}