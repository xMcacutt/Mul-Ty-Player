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
        private bool _doKoalaCollision;
        public bool DoKoalaCollision
        {
            get { return _doKoalaCollision; }
            set
            {
                _doKoalaCollision = value;
                // Update the settings file here
                SettingsHandler.Settings.DoKoalaCollision = value;
            }
        }

        public SettingsViewModel()
        {
        }

        public void Setup()
        {
            DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
        }
    }
}