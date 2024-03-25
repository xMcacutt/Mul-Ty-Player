using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandFreeCam : Command
{
    public static bool inFreeCam;
    
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
        ProcessHandler.TryRead(0x27EBD0, out int currentCameraState, true, "ReadCamState");
        inFreeCam = currentCameraState == 28;
        var newCamState = inFreeCam ? 5 : 28;
        var newHeroState = 5;
        if (Client.HLevel.CurrentLevelData.Id == Levels.OutbackSafari.Id)
        {
            newHeroState = inFreeCam ? 0 :  5;
        }
        else
        {
            newHeroState = inFreeCam ? 35 :  50;
        }
        
        if (inFreeCam)
        {
            var camPos = SpectatorHandler.ReadCameraPosition();
            Client.HHero.WritePosition(camPos.X, camPos.Y, camPos.Z);
        }
        Client.HGameState.SetCameraState(newCamState);
        Client.HHero.SetHeroState(newHeroState);
        if (!inFreeCam)
            SpectatorHandler.SetCameraRotation(0, Client.HHero.GetCurrentPosRot()[4]);
    }
}