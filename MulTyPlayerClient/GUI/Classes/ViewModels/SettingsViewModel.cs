using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public bool DoKoalaCollision { get; set; }

        public bool AutoRestartTy { get; set; }

        public void Setup()
        {
            DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
            AutoRestartTy = SettingsHandler.Settings.AutoRestartTyOnCrash;
        }

        public void Save()
        {
            SettingsHandler.Settings.AutoRestartTyOnCrash = AutoRestartTy;
            SettingsHandler.Settings.DoKoalaCollision = DoKoalaCollision;
        }
    }
}