using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class MainClientViewModel
    {
        public List<PlayerInfo> PlayerInfoList { get; set; }

        public MainClientViewModel()
        {
            PlayerInfoList = new List<PlayerInfo>();
        }
    }
}

