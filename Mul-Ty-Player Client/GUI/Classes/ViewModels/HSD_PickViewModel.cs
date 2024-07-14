using System;
using System.Windows.Media;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.Views;
using PropertyChanged;
using HSD_Pick = MulTyPlayerClient.GUI.Models.HSD_Pick;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_PickViewModel
{
    public bool IsPick { get; set; }
    public bool IsBan { get; set; }
    public SolidColorBrush TeamColor { get; set; }
    public Uri BanSource { get; set; }
    public Uri PickSource { get; set; }

    public HSD_PickModel PickModel;
    
    public HSD_PickViewModel(HSD_PickModel pickModel)
    {
        PickModel = pickModel;
        BanSource = pickModel.BanSource;
        PickSource = pickModel.PickSource;
        var pick = pickModel.Pick;
        switch (pick)
        {
            case HSD_Pick.Pick:
                IsPick = true;
                IsBan = false;
                break;
            case HSD_Pick.Ban:
                IsPick = false;
                IsBan = true;
                break;
            default:
                break;
        }
        if (pickModel.Team == HSD_Team.Team1)
            TeamColor = App.AppColors.MainAccent;
        if (pickModel.Team == HSD_Team.Team2)
            TeamColor = App.AppColors.AltAccent;
    }
}