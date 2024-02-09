using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.Views;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class KoalaSelectViewModel : IViewModel
{
    public KoalaSelectViewModel()
    {
        KoalaSelect.OnKoalaSelected += delegate { KoalaSelect.BlockKoalaSelect = true; };
        KoalaSelect.OnProceedToLobby += delegate { KoalaSelect.BlockKoalaSelect = false; };
        KoalaSelect.OnBlockKoalaSelectChanged += Model_BlockKoalaSelectChanged;
        Boonie = new KoalaSelectEntryViewModel(KoalaSelect.Boonie);
        Mim = new KoalaSelectEntryViewModel(KoalaSelect.Mim);
        Gummy = new KoalaSelectEntryViewModel(KoalaSelect.Gummy);
        Snugs = new KoalaSelectEntryViewModel(KoalaSelect.Snugs);
        Katie = new KoalaSelectEntryViewModel(KoalaSelect.Katie);
        Kiki = new KoalaSelectEntryViewModel(KoalaSelect.Kiki);
        Elizabeth = new KoalaSelectEntryViewModel(KoalaSelect.Elizabeth);
        Dubbo = new KoalaSelectEntryViewModel(KoalaSelect.Dubbo);
    }

    public KoalaSelectEntryViewModel Boonie { get; set; }
    public KoalaSelectEntryViewModel Mim { get; set; }
    public KoalaSelectEntryViewModel Gummy { get; set; }
    public KoalaSelectEntryViewModel Snugs { get; set; }
    public KoalaSelectEntryViewModel Katie { get; set; }
    public KoalaSelectEntryViewModel Kiki { get; set; }
    public KoalaSelectEntryViewModel Elizabeth { get; set; }
    public KoalaSelectEntryViewModel Dubbo { get; set; }

    public bool BlockKoalaSelect { get; set; }
    
    private void Model_BlockKoalaSelectChanged(bool value)
    {
        BlockKoalaSelect = value;
    }

    private static KoalaSelectModel KoalaSelect => ModelController.KoalaSelect;

    public void OnEntered()
    {
    }

    public void OnExited()
    {
    }
}