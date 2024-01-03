using System;
using System.Threading;
using Riptide;
using Riptide.Utils;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Sync;

namespace MulTyPlayerServer;

internal class Server
{
    public const int TICK_RATE_HZ = 50;
    public const int MS_PER_TICK = 1000 / TICK_RATE_HZ;

    public static Riptide.Server _Server;
    public static bool _isRunning;

    private static KoalaHandler HKoala => Program.HKoala;

    public static void StartServer()
    {
        RiptideLogger.Initialize(Console.WriteLine, true);
        _isRunning = true;

        new Thread(Loop).Start();
    }

    private static void Loop()
    {
        _Server = new Riptide.Server();
        _Server.Start(SettingsHandler.Settings.Port, 8);

        _Server.HandleConnection += HandleConnection;
        _Server.ClientConnected += ClientConnected;
        _Server.ClientDisconnected += ClientDisconnected;

        while (_isRunning)
        {
            _Server.Update();
            if (PlayerHandler.Players.Count != 0)
            {
                if (SettingsHandler.DoLevelLock)
                    LevelLockHandler.UpdateCheck();
                foreach (var player in PlayerHandler.Players.Values)
                {
                    if (player.CurrentLevel != player.PreviousLevel)
                    {
                        HKoala.ReturnKoala(player);
                        player.PreviousLevel = player.CurrentLevel;
                    }

                    SendCoordinatesToAll(player.ClientID, player.Koala.KoalaName, player.CurrentLevel,
                        player.Coordinates, player.OnMenu);
                }
            }
            Thread.Sleep(MS_PER_TICK);
        }

        if (Program._inputStr == "y") return;
        _Server.Stop();
        Console.WriteLine("Would you like to restart Mul-Ty-Player? [y/n]");
    }

    public static void RestartServer()
    {
        Program._inputStr = "y";
        _Server.Stop();
        _isRunning = false;
    }

    private static void HandleConnection(Connection pendingConnection, Message authenticationMessage)
    {
        Console.WriteLine("Received connection attempt...");
        var pass = authenticationMessage.GetString();
        if (!string.Equals(pass, SettingsHandler.Settings.Password, StringComparison.CurrentCultureIgnoreCase)
            && !string.Equals(SettingsHandler.Settings.Password, "XXXXX", StringComparison.CurrentCultureIgnoreCase)
            && !string.IsNullOrWhiteSpace(SettingsHandler.Settings.Password)
            && _Server.ClientCount > 0)
        {
            _Server.Reject(pendingConnection);
            Console.WriteLine("Rejecting.");
        }
        else
        {
            _Server.Accept(pendingConnection);
            Console.WriteLine("Accepting.");
        }
    }

    private static void ClientConnected(object sender, ServerConnectedEventArgs e)
    {
        //Console.WriteLine("Client connected.");
        KoalaHandler.SendKoalaAvailability(e.Client.Id);
        SettingsHandler.SendSettings(e.Client.Id);
    }

    private static void ClientDisconnected(object sender, ServerDisconnectedEventArgs e)
    {
        if (PlayerHandler.Players.TryGetValue(e.Client.Id, out _))
        {
            SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Name} has disconnected from the server.", true);
            SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Koala.KoalaName} was returned to the koala pool",
                true);
            if (PlayerHandler.Players[e.Client.Id].Koala != null)
                HKoala.ReturnKoala(PlayerHandler.Players[e.Client.Id]);
            PlayerHandler.RemovePlayer(e.Client.Id);
            PlayerHandler.AnnounceDisconnect(e.Client.Id);
        }
    }

    public static void SendCoordinatesToAll(ushort clientID, string koalaName, int level, float[] coordinates,
        bool onMenu)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.KoalaCoordinates);
        message.AddBool(onMenu);
        message.AddUShort(clientID);
        message.AddString(koalaName);
        message.AddInt(level);
        message.AddFloats(coordinates);
        _Server.SendToAll(message, clientID);
    }

    public static void SendMessageToClient(string str, bool printToServer, ushort to)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
        message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
        _Server.Send(message, to);
        if (printToServer) Console.WriteLine(str);
    }

    public static void SendMessageToClients(string str, bool printToServer, ushort except)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
        message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
        _Server.SendToAll(message, except);
        if (printToServer) Console.WriteLine(str);
    }

    public static void SendMessageToClients(string str, bool printToServer)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConsoleSend);
        message.AddString($"[{DateTime.Now:HH:mm:ss}] (SERVER) {str}");
        _Server.SendToAll(message);
        if (printToServer) Console.WriteLine(str);
    }
}