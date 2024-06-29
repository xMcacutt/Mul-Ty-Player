using System.Collections.ObjectModel;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HS_ChooseTeamViewModel : IViewModel
{
    public void OnEntered()
    {
    }

    public void OnExited()
    {
    }

    public ObservableCollection<string> Team1NamesCollection { get; set; }
    public ObservableCollection<string> Team2NamesCollection { get; set; }

    public HS_ChooseTeamViewModel()
    {
        Team1NamesCollection = new ObservableCollection<string>();
    }
}