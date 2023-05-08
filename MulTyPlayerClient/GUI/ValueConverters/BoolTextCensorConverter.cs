using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace MulTyPlayerClient.GUI
{
    /// <summary>
    /// A converter that takes in a string and bool to decide if to hide it or not/>
    /// </summary>
    public class BoolTextCensorConverter : BaseValueConverter<BoolTextCensorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Regex("\\s|\\S").Replace(value.ToString(), "*");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}