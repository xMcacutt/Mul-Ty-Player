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

        KoalaSelectModel model;

        public KoalaSelectViewModel()
        {
            model = ModelController.KoalaSelect;
            model.MakeAllAvailable();
            model.OnKoalaSelected += delegate { BlockKoalaSelect = true; };
            model.OnProceedToLobby += delegate { BlockKoalaSelect = false; };
            Boonie = new (model.Boonie);
            Mim = new (model.Mim);
            Gummy = new (model.Gummy);
            Snugs = new (model.Snugs);
            Katie = new (model.Katie);
            Kiki = new (model.Kiki);
            Elizabeth = new (model.Elizabeth);
            Dubbo = new (model.Dubbo);
        }

        public void OnEntered()
        {
        }

        public void OnExited()
        {
        }
    }
}
