using System.Windows;

namespace MulTyPlayerClient.GUI.Classes.Views;

public partial class HSD_PerkWindow : Window
{
    public HSD_PerkWindow()
    {
        InitializeComponent();
    }

    private void Perk_OnClick(object sender, RoutedEventArgs e)
    {
        if (Client.HHideSeek.Role == HSRole.Hider)
        Client.HHideSeek.CurrentPerk = PerkHandler
    }
    
    private void Debuff_OnClick(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}