using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

public class MtpCommandRequestHost : Command
{
    public MtpCommandRequestHost()
    {
        Name = "requesthost";
        Aliases = new List<string> { "rh", "reqhost" };
        HostOnly = false;
        Usages = new List<string> { "/requesthost" };
        Description = "Request host privileges if no host already exists.";
        ArgDescriptions = new Dictionary<string, string>();
    }

    public override void InitExecute(string[] args)
    {
        if (args.Length != 0)
        {
            SuggestHelp();
            return;
        }
        if (PlayerHandler.Players[Client._client.Id].IsHost)
        {
            LogError("You already have host privileges");
            return;
        }
        RequestHost();
    }
    
    private static void RequestHost()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqHost);
        Client._client.Send(message);
    }
    
    [MessageHandler((ushort)MessageID.ReqHost)]
    public static void RequestHostResponse(Message message)
    {
        if (message.GetBool())
        {
            PlayerHandler.Players[Client._client.Id].IsHost = true;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(ModelController.Lobby.UpdateHostIcon));
            Logger.Write("You have been made host. You now have access to host only commands.");
            return;
        }
        try
        {
            var existingHost = PlayerHandler.Players.Values.First(p => p.IsHost);
            Logger.Write($"Player {existingHost.Name} already has host privileges");
        }
        catch
        {
            Logger.Write("Existing host found by server, not by client. Denying host privileges.");
            
        }
    }
    
    [MessageHandler((ushort)MessageID.HostChange)]
    public static void HostChange(Message message)
    {
        foreach (var key in PlayerHandler.Players.Keys) PlayerHandler.Players[key].IsHost = false;
        PlayerHandler.Players[message.GetUShort()].IsHost = true;
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Lobby.UpdateHostIcon));
    }
}