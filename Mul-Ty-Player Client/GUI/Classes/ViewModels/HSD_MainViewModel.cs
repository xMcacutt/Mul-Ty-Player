using System;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_MainViewModel
{
    public IViewModel CurrentViewModel { get; set; }
    private IViewModel lastViewModel;
    private readonly HS_ChooseTeamViewModel chooseTeamViewModel;
    private readonly HS_DraftViewModel draftViewModel;

    public HSD_MainViewModel()
    {
        chooseTeamViewModel = new HS_ChooseTeamViewModel();
        draftViewModel = new HS_DraftViewModel();
        
        lastViewModel = chooseTeamViewModel;
        CurrentViewModel = chooseTeamViewModel;
        
        ModelController.HS_ChooseTeam.OnChosenTeam += () => GoToView(HSD_View.Draft);
    }
    
    private void GoToView(HSD_View view)
    {
        lastViewModel = CurrentViewModel;
        CurrentViewModel = view switch
        {
            HSD_View.ChooseTeam => chooseTeamViewModel,
            HSD_View.Draft => draftViewModel,
            _ => throw new NotImplementedException($"Tried to switch to unsupported view: {view}")
        };
        lastViewModel.OnExited();
        CurrentViewModel.OnEntered();
    }
}

public enum HSD_View
{
    ChooseTeam,
    Draft
}