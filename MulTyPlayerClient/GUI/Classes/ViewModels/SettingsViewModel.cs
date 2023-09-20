using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        public bool DoKoalaCollision { get; set; }
        public bool AutoRestartTy { get; set; }

        public void SetCheckboxesFromSettings()
        {
            DoKoalaCollision = SettingsHandler.Settings.DoKoalaCollision;
            AutoRestartTy = SettingsHandler.Settings.AutoRestartTyOnCrash;
        }
    }
}