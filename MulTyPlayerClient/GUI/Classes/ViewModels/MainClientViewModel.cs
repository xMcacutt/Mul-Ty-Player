using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class MainClientViewModel
    {
        public List<PlayerInfo> PlayerInfoList { get; set; }
        public ICommand ManageInputCommand { get; set; }
        public string Input { get; set; }

        public MainClientViewModel()
        {
            PlayerInfoList = new List<PlayerInfo>();
            ManageInputCommand = new RelayCommand(ManageInput);
        }

        public void ResetPlayerList()
        {
            PlayerInfoList.Clear();
        }

        public void ManageInput()
        {
            if (string.IsNullOrWhiteSpace(Input)) return;
            if (Input.StartsWith('/'))
            {
                Client.HCommand.ParseCommand(Input);
            }
            else BasicIoC.LoggerInstance.Write(Input);
            Input = null;
        }
    }
}

