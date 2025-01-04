using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using MulTyPlayerClient;
using Riptide;
using Client = MulTyPlayerClient.Client;

namespace MulTyPlayerClient;

public class MtpCommandDevPass : Command
{
    public MtpCommandDevPass()
    {
        Name = "devpass";
        Aliases = new List<string> { "dp", "dev" };
        Description = "Ask for dev features.";
        Usages = new List<string> { "/devpass" };
        ArgDescriptions = new Dictionary<string, string>();
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length == 0)
            RunDevPass();
        else
            SuggestHelp();
    }
    
    public string RunDevPass()
    {
        if (SettingsHandler.ClientSettings.DevPass != "")
            Client._client.Send(Message.Create(MessageSendMode.Reliable, MessageID.DevPass)
                .AddString(SettingsHandler.ClientSettings.DevPass));
        return "";
    }
    
    
    [MessageHandler((ushort)MessageID.DevPass)]
    public static void HandleDevCheck(Message message)
    {
        Client.HCommand.DevPassed = message.GetBool();
        Logger.Write(Client.HCommand.DevPassed
            ? "[INFO] You now have access to developer features."
            : "[ERROR] You are not a developer.");
    }
}