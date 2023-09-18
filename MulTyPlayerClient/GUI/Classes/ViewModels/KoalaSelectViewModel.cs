using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class KoalaSelectViewModel : IViewModel
    {
        public KoalaSelectEntryViewModel Boonie { get; set; }
        public KoalaSelectEntryViewModel Mim { get; set; }
        public KoalaSelectEntryViewModel Gummy { get; set; }
        public KoalaSelectEntryViewModel Snugs { get; set; }
        public KoalaSelectEntryViewModel Katie { get; set; }
        public KoalaSelectEntryViewModel Kiki { get; set; }
        public KoalaSelectEntryViewModel Elizabeth { get; set; }
        public KoalaSelectEntryViewModel Dubbo { get; set; }

        public bool BlockKoalaSelect { get; set; }

        private static KoalaSelectModel KoalaSelect => ModelController.KoalaSelect;

        public KoalaSelectViewModel()
        {
            KoalaSelect.OnKoalaSelected += delegate { BlockKoalaSelect = true; };
            KoalaSelect.OnProceedToLobby += delegate { BlockKoalaSelect = false; };
            Boonie = new (KoalaSelect.Boonie);
            Mim = new (KoalaSelect.Mim);
            Gummy = new (KoalaSelect.Gummy);
            Snugs = new (KoalaSelect.Snugs);
            Katie = new (KoalaSelect.Katie);
            Kiki = new (KoalaSelect.Kiki);
            Elizabeth = new (KoalaSelect.Elizabeth);
            Dubbo = new (KoalaSelect.Dubbo);
        }

        public void OnEntered()
        {
        }

        public void OnExited()
        {
        }
    }
}
