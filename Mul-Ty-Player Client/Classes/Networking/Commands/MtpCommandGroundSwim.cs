using System.Collections.Generic;
using MulTyPlayerClient.Classes.Networking;

namespace MulTyPlayerClient;

public class MtpCommandGroundSwim : Command
{
    public MtpCommandGroundSwim()
    {
        Name = "groundswim";
        Aliases = new List<string> { "gs", "swim" };
        HostOnly = false;
        SpectatorAllowed = false;
        Usages = new List<string> { "/groundswim" };
        Description = "Attempts to start player swimming.";
        ArgDescriptions = new Dictionary<string, string>
        {
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length > 1)
        {
            SuggestHelp();
            return;
        }
        if (Client.HGameState.IsOnMainMenuOrLoading)
        {
            LogError("Cannot change hero state on main menu or load screen.");
            return;
        }
        if (Client.HLevel.CurrentLevelId == Levels.OutbackSafari.Id)
        {
            LogError("Cannot change hero state to swimming in Outback Safari.");
            return;
        }
        // I DONT KNOW WHY THIS NEEDS TO BE INVERTED... IT SHOULDN'T... BUT IT DOES
        if (!MtpCommandFreeCam.inFreeCam)
        {
            LogError("Cannot change hero state to swimming while in freecam.");
            return;
        }
        
        if (args.Length == 0)
            RunGroundSwim();
    }

    private void RunGroundSwim()
    {
        Client.HHero.SetHeroState(39);
    }
}