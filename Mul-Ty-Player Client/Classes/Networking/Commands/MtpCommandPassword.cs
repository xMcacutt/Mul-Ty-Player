using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandPassword : Command
{
    public MtpCommandPassword()
    {
        Name = "password";
        Aliases = new List<string> { "pass", "pwd" };
        Description = "Set or get the server password.";
        Usages = new List<string> { "/password", "/password <password>", "/password default" };
        HostOnly = true;
        SpectatorAllowed = true;
        ArgDescriptions = new Dictionary<string, string>
        {
            { "<password>", "New password for server." }
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length > 1)
        {
            SuggestHelp();
            return;
        }
        if (args.Length == 0)
        {
            RunPassword();
            return;
        }
        var password = args[0];
        if ((!password.All(char.IsLetter) || password.Length != 5) && !string.Equals(password, "default", StringComparison.CurrentCultureIgnoreCase))
        {
            LogError($"{password} is not a valid password. It must be exactly 5 LETTERS.");
            return;
        }
        RunPassword(password);
    }

    public void RunPassword()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.GetPassword);
        Client._client.Send(message);
    }

    public void RunPassword(string newPassword)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SetPassword);
        message.AddString(newPassword);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.SetPassword)]
    private static void NewPasswordSet(Message message)
    {
        var pass = message.GetString();
        Logger.Write(
            string.Equals(pass, "xxxxx", StringComparison.CurrentCultureIgnoreCase) ?
            "Password set to default.\nPassword check will be bypassed for connecting players." : 
            $"Password changed to {pass}");
    }
}