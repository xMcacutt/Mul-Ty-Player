using System;
using System.Drawing;
using System.Windows.Media;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using Color = System.Windows.Media.Color;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_PlayerViewModel
{
    public HSD_PlayerModel PlayerModel;

    public HSD_PlayerViewModel(HSD_PlayerModel? playerModel, HSD_Team team)
    {
        PlayerModel = playerModel;
        Koala = PlayerModel.Koala;
        KoalaImageSource = PlayerModel.KoalaImageSource;
        PlayerName = PlayerModel.PlayerName;
        if (team == HSD_Team.Team1)
            TeamColor = App.AppColors.MainAccent;
        if (team == HSD_Team.Team2)
            TeamColor = App.AppColors.AltAccent;
    }

    public SolidColorBrush TeamColor { get; set; }
    public string PlayerName { get; set; }
    public KoalaInfo Koala { get; set; }
    public Uri KoalaImageSource { get; set; }
}