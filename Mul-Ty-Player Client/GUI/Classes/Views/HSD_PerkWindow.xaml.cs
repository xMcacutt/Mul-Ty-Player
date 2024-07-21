using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Classes.Views;

public partial class HSD_PerkWindow : Window
{
    public HSD_PerkWindow()
    {
        InitializeComponent();
    }

    private void Perk_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HHideSeek.CurrentPerk = Client.HHideSeek.Role == HSRole.Hider ? 
            PerkHandler.HiderPerks
                .First(x => x.DisplayName == (sender as Button).Content.ToString()) 
            : PerkHandler.SeekerPerks
                .First(x => x.DisplayName == (sender as Button).Content.ToString());
        (DataContext as HSD_PerkWindowViewModel).ChoosePerkActive = false;
    }
    
    private void Debuff_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HHideSeek.CurrentDebuff = Client.HHideSeek.Role == HSRole.Hider ? 
            PerkHandler.HiderDebuffs
                .First(x => x.DisplayName == (sender as Button).Content.ToString()) 
            : PerkHandler.SeekerDebuffs
                .First(x => x.DisplayName == (sender as Button).Content.ToString());
        Client.HPlayer.SetReady();
        Close();
    }
}