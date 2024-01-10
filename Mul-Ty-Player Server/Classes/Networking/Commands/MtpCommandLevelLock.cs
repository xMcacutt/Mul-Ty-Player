using System;
using System.Collections.Generic;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandLevelLock : Command
{

    public MtpCommandLevelLock()
    {
        Name = "levellock";
        Aliases = new List<string> { "ll", "llock" };
        Description = "Activate / Deactivate Level Lock mode.";
        Usages = new List<string> { "/levellock <true/false>" };
        ArgDescriptions = new Dictionary<string, string>()
        {
            { "<true/false>", "Whether Level Lock mode should be turned on or off."}
        };
    }

    public override string InitExecute(string[] args)
    {
        if (!SettingsHandler.SyncSettings["TE"] || !SettingsHandler.SyncSettings["Cog"] ||
            !SettingsHandler.SyncSettings["Attribute"])
            return LogError("Server not set up for Level Lock mode. ThunderEgg, Cog, and Attribute syncing must be enabled.");
        if (args.Length != 1)
            return SuggestHelp();
        var doLevelLock = args[0] == "true" ? true : (args[0] == "false" ? false : (bool?)null);
        return doLevelLock == null ? SuggestHelp() : RunLevelLock((bool)doLevelLock);
    }
    
    private static string RunLevelLock(bool value)
    {
        SettingsHandler.DoLevelLock = value;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SetLevelLock);
        message.AddBool(value);
        Server._Server.SendToAll(message);
        return $"Level Lock mode set to {value}";
    }

    [MessageHandler((ushort)MessageID.SetLevelLock)]
    private static void ProxyRunLevelLock(ushort fromClientId, Message message)
    {
        RunLevelLock(message.GetBool());
    }
}