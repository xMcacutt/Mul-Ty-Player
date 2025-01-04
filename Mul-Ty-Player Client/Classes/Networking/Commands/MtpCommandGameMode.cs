using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandGameMode : Command
{
    private GameMode[] devGMs = new[] { GameMode.Blitz };
    
    public MtpCommandGameMode()
    {
        Name = "gamemode";
        Aliases = new List<string> { "gm", "mode" };
        Usages = new List<string> { "/gamemode <mode>" };
        Description = "Change game mode";
        HostOnly = true;
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<mode>", "Mode to switch to { Normal, HideSeek, Chaos, Collection }"}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length is not 1 || !Enum.TryParse(args[0], true, out GameMode mode))
            SuggestHelp();
        else if (devGMs.Contains(mode) && !Client.HCommand.DevPassed)
            Logger.Write("[ERROR] You are not a developer.");
        else
            RunGameMode(mode);
    }
    
    public void RunGameMode(GameMode mode)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.GameMode);
        message.AddInt((int)mode);
        Client._client.Send(message);
        Logger.Write("Requested game mode change...");
    }
}