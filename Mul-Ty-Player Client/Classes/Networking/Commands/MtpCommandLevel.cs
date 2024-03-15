using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandLevel : Command
{
    public MtpCommandLevel()
    {
        Name = "level";
        Aliases = new List<string> { "lvl", "warp" };
        HostOnly = false;
        Usages = new List<string> { "/level <levelId>", "/level <levelId> <warpAll>" };
        Description = "Warp to a specific level.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<levelId>", "Level to warp to."},
            {"<warpAll>", "Whether to warp other players too"}
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1 && args.Length != 2)
        {
            SuggestHelp();
            return;
        }

        if (Client.HGameState.IsOnMainMenuOrLoading)
        {
            LogError("Cannot change level from main menu or while loading.");
            return;
        }

        int level;
        var levelCodeSpecified = Levels.levels.Values.Any(x => string.Equals(args[0], x.Code, StringComparison.CurrentCultureIgnoreCase));
        if (levelCodeSpecified)
            level = Levels.levels.Values.First(x => string.Equals(args[0], x.Code, StringComparison.CurrentCultureIgnoreCase)).Id;
        else if (!int.TryParse(args[0], out level))
        {
            LogError("Invalid level specified.");
            return;
        }
        if (level == 16)
        {
            LogError("Cannot warp to endgame.");
            return;
        }
        if (!Levels.levels.ContainsKey(level))
        {
            LogError("Invalid level specified.");
            return;
        }
        var warpAll = false;
        if (args.Length == 2)
        {
            if (!string.Equals(args[1], "true", StringComparison.CurrentCultureIgnoreCase)
                && !string.Equals(args[1], "false", StringComparison.CurrentCultureIgnoreCase))
            {
                LogError("Invalid second argument. Should be true / false.");
                return;
            }
            warpAll = string.Equals(args[1], "true", StringComparison.CurrentCultureIgnoreCase);
        }
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            LogError("Critical error. Could not find self.");
            return;
        }
        if (warpAll && !self.IsHost)
        {
            LogError("Warping all is a host only privilege.");
            return;
        }
        RunLevel(level, warpAll);
    }

    private void RunLevel(int level, bool warpAll)
    {
        Client.HLevel.ChangeLevel(level);
        if (!warpAll) 
            return;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ForceLevelChange);
        message.AddInt(level);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.ForceLevelChange)]
    private static void HandleForceLevelChange(Message message)
    {
        var level = message.GetInt();
        Client.HLevel.ChangeLevel(level);
    }
    
}