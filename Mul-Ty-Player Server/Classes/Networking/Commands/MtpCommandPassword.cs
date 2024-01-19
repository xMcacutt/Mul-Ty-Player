using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking.Commands;

public class MtpCommandPassword : Command
{
    public MtpCommandPassword()
    {
        Name = "password";
        Aliases = new List<string> { "pass", "pwd" };
        Description = "Set or get the server password.";
        Usages = new List<string> { "/password", "/password <password>" };
        ArgDescriptions = new Dictionary<string, string>
        {
            { "<password>", "New password for server." }
        };
    }
    
    public override string InitExecute(string[] args)
    {
        if (args.Length == 0)
            return RunPassword();
        if (args.Length != 1)
            return SuggestHelp();
        var password = args[0]; 
        if (!password.All(char.IsLetter) || password.Length != 5)
            return LogError($"{password} is not a valid password. A password must be exactly 5 LETTERS.");
        return RunPassword(args[0]);
    }

    private static string RunPassword()
    {
        return $"The current password is {SettingsHandler.Settings.Password}\nTo set a new password use /password <password>";
    }
    
    private static string RunPassword(string password)
    {
        SettingsHandler.Settings.Password = password.ToUpper();
        return $"{SettingsHandler.Settings.Password} set as new password.";
    }
    
    [MessageHandler((ushort)MessageID.SetPassword)]
    public static void ProxyRunSetPassword(ushort clientId, Message message)
    {
        var pass = message.GetString();
        if (string.Equals(pass, "default", StringComparison.CurrentCultureIgnoreCase))
            pass = "xxxxx";
        RunPassword(pass);
        var announcement = Message.Create(MessageSendMode.Reliable, MessageID.SetPassword);
        announcement.AddString(pass);
        Server._Server.SendToAll(announcement);
    }
    
    [MessageHandler((ushort)MessageID.GetPassword)]
    public static void ProxyRunGetPassword(ushort clientId, Message message)
    {
        PeerMessageHandler.SendMessageToClient(RunPassword(), false, clientId);
    }
}