using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        private bool _doKoalaCollision;
        private bool _autoRestartTy;
        public bool DoKoalaCollision
        {
            get { return _doKoalaCollision; }
            set
            {
                _doKoalaCollision = value;
                // Update the settings file here
                SettingsHandler.Settings.DoKoalaCollision = value;
                SettingsHandler.Save();
            }
        }

        public bool AutoRestartTy
        {
            get { return _autoRestartTy; }
            set
            {
                _autoRestartTy = value;
                // Update the settings file here
                SettingsHandler.Settings.AutoRestartTyOnCrash = value;
                SettingsHandler.Save();
            }
        }

        public void Setup()
        {
            DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
            AutoRestartTy = SettingsHandler.Settings.AutoRestartTyOnCrash;
        }
    }
}