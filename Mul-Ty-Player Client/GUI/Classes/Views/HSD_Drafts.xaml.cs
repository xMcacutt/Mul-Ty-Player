using System.Windows;
using System.Windows.Controls;
using MulTyPlayer;
using Octokit;
using Riptide;

namespace MulTyPlayerClient.GUI.Views;

public partial class HSD_Drafts : UserControl
{
    public HSD_Drafts()
    {
        InitializeComponent();
    }

    private void SwapTeam1Click(object sender, RoutedEventArgs e)
    {
        if (HSD_DraftsHandler.Team1.Count == 4)
            return;
        Client.HDrafts.LeaveTeam();
        Client.HDrafts.RequestJoinTeam(HSD_Team.Team1);
    }

    private void SwapTeam2Click(object sender, RoutedEventArgs e)
    {
        if (HSD_DraftsHandler.Team2.Count == 4)
            return;
        Client.HDrafts.LeaveTeam();
        Client.HDrafts.RequestJoinTeam(HSD_Team.Team2);
    }

    private void StartSessionClick(object sender, RoutedEventArgs e)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_Start);
        Client._client.Send(message);
    }
}