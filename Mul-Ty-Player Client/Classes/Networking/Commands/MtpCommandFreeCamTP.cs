using System;
using System.Collections.Generic;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandFreeCam : Command
{
    public MtpCommandFreeCam()
    {
        Name = "freecam";
        Aliases = new List<string> { "fc", "cam" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/freecam" };
        Description = "Puts the player into freecam mode. Exiting the mode causes the player to teleport to the position of the camera.";
        ArgDescriptions = new Dictionary<string, string>
        {
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length is not 0 and not 2)
        {
            SuggestHelp();
            return;
        }
        
        if (args.Length == 0 && !ModelController.Login.JoinAsSpectator)
            RunFreecam();
        else if (!string.Equals(args[0], "speed", StringComparison.CurrentCultureIgnoreCase)
                 || !float.TryParse(args[1], out var speed))
            SuggestHelp();
        else
            SpectatorHandler.SetFreecamSpeed(speed, true);
    }

    private void RunFreecam()
    {
        SpectatorHandler.ToggleFreeCam();
    }
}