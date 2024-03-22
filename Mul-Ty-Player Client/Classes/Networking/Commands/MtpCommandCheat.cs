using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

public class MtpCommandCheat : Command
{
    private static List<string> _cheatIds = new List<string>() {
        "lines",
        "menu",
        "techno",
        "elemental",
        "invincibility",
        "l",
        "m",
        "t",
        "e",
        "i",
    };
    
    public MtpCommandCheat()
    {
        Name = "cheat";
        Aliases = new List<string> { "ch" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/ch <cheatId>" };
        Description = "Toggles the specified cheat.";
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "<cheatId>", "Cheat identifier. Must be any of [lines, menu, techno, elemental, invincibility] or their initial." }
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1)
        {
            SuggestHelp();
            return;
        }
        var cheatType = args[0];
        if (!_cheatIds.Contains(cheatType, StringComparer.CurrentCultureIgnoreCase))
        {
            SuggestHelp();
            return;
        }
        RunCheat(cheatType);
    }

    public void RunCheat(string cheatType)
    {
        switch (cheatType[..1].ToLower())
        {
            // Elemental-Rangs
            case "e":
                Client.HHero.ToggleElementals();
                break;
            // Lines In Sky
            case "l":
                Client.HGameState.ToggleCollectibleLines();
                break;
            // Techno-Rangs
            case "t":
                Client.HHero.GiveTechnorangs();
                break;
            // Level Select Menu
            case "m":
                Client.HGameState.ToggleLevelSelect();
                break;
            // Invincibility 
            case "i":
                Client.HHero.ToggleInvincibility();
                break;
            default:
                return;
        }
        SFXPlayer.PlaySound(SFX.CheatActivated);
    }
}
