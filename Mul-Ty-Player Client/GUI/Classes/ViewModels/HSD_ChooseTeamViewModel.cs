using System.Collections.ObjectModel;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_ChooseTeamViewModel : IViewModel
{
    public void OnEntered()
    {
    }

    public void OnExited()
    {
    }

    public HSD_ChooseTeamViewModel()
    {
    }

    public void OnTeamSelected(int team)
    {
        ModelController.HSD_ChooseTeam.OnTeamSelected(team);
    }
}