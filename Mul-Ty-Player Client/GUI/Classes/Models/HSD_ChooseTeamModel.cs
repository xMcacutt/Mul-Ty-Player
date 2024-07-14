using System;

namespace MulTyPlayerClient.GUI.Models;

public class HSD_ChooseTeamModel
{
    public event Action OnChosenTeam;

    public void OnTeamSelected(int team)
    {
        Client.HDrafts.RequestJoinTeam((HSD_Team)team);
        OnChosenTeam.Invoke();
    }
}