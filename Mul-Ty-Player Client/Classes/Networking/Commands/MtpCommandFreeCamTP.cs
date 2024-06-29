using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandFreeCam : Command
{
    public MtpCommandFreeCam()
    {
        Name = "freecam";
        Aliases = new List<string> { "fc", "cam" };
        HostOnly = false;
        SpectatorAllowed = false;
        Usages = new List<string> { "/freecam" };
        Description = "Puts the player into freecam mode. Exiting the mode causes the player to teleport to the position of the camera.";
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
        RunFreecam();
    }

    private void RunFreecam()
    {
        SpectatorHandler.ToggleFreeCam();
    }
}