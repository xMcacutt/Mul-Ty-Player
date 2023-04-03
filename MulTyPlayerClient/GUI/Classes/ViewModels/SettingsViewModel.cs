using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public bool DoKoalaCollision { get; set; }
        public ICommand CheckBoxClickedCommand { get; set; }

        public SettingsViewModel()
        {
            CheckBoxClickedCommand = new RelayCommand(CheckBoxClicked);
        }

        public void Setup()
        {
            DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
        }

        public void CheckBoxClicked()
        {
            SettingsHandler.Settings.DoKoalaCollision = DoKoalaCollision;
        }
    }
}