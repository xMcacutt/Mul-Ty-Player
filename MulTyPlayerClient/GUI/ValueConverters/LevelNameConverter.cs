using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI
{
    public class LevelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string level = value as string;
            return level switch
            {
                "Z1" => "Rainbow Cliffs",
                "Z2" => "???",
                "Z3" => "???",
                "Z4" => "???",
                "A1" => "Two Up",
                "A2" => "Walk in the Park",
                "A3" => "Ship Rex",
                "A4" => "Bull's Pen",
                "B1" => "Bridge on the River Ty",
                "B2" => "Snow Worries",
                "B3" => "Outback Safari",
                "B4" => "Kumu Caves?!?!",
                "C1" => "Lyre, Lyre Pants on Fire",
                "C2" => "Beyond the Black Stump",
                "C3" => "Rex Marks the Spot",
                "C4" => "Fluffy's Fjord",
                "D1" => "???",
                "D2" => "Cass' Crest",
                "D3" => "???",
                "D4" => "Crikey's Cove",
                "E1" => "Cass' Pass",
                "E2" => "Bonus World 1",
                "E3" => "Bonus World 2",
                "E4" => "Final Battle",
                "END" => "Credits",
                "Menu" => "On Main Menu",
                _ => "Not In Level",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
