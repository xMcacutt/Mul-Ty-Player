using System.Windows;
using System.Windows.Controls;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

public partial class HSD_ChooseTeam : UserControl
{
    public HSD_ChooseTeam()
    {
        InitializeComponent();
    }

    private void Team1_OnClick(object sender, RoutedEventArgs e)
    {
        (DataContext as HSD_ChooseTeamViewModel).OnTeamSelected(1);
    }

    private void Team2_OnClick(object sender, RoutedEventArgs e)
    {
        (DataContext as HSD_ChooseTeamViewModel).OnTeamSelected(2);
    }
}