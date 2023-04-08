using Steamworks;
using System;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Shapes;

namespace MulTyPlayerClient.GUI
{
    public class StringsToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Check that both values are not null or empty strings
            string tyPath = values[0] as string;
            string mtpPath = values[1] as string;
            if (!string.IsNullOrEmpty(tyPath) && !string.IsNullOrEmpty(mtpPath))
            {
                // Check that the paths are valid folders
                if (System.IO.Directory.Exists(tyPath) && System.IO.Directory.Exists(mtpPath) && CheckTyFolder(tyPath) && CheckMTPPath(mtpPath))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckMTPPath(string mtpPath)
        {
            if(Directory.GetFiles(mtpPath).Length == 0) { return true; }
            if(File.Exists(System.IO.Path.Combine(mtpPath, "Patch_PC.rkv"))) { return true; }
            return false;
        }

        private bool CheckTyFolder(string tyPath)
        {
            int i = 0;
            foreach (string fileName in BasicIoC.SetupViewModel.TyFileNames)
            {
                string filePath = System.IO.Path.Combine(tyPath, fileName);
                FileInfo fileInfo = new FileInfo(filePath);
                int index = Array.IndexOf(BasicIoC.SetupViewModel.TyFileNames, fileName);
                if (fileInfo.Exists)
                {
                    i++;
                }
            }
            if (i == BasicIoC.SetupViewModel.TyFileNames.Length)
            {
                return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
