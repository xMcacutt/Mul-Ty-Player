﻿using System;
using System.Linq;
using System.Threading;
using Riptide;
using Riptide.Utils;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Networking.Commands;
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
        _Server.Start(SettingsHandler.ServerSettings.Port,12);

        _Server.HandleConnection += HandleConnection;
        _Server.ClientConnected += ClientConnected;
        _Server.ClientDisconnected += ClientDisconnected;

        VoiceServer.OpenVoiceServer();
        
        while (_isRunning)
        {
            _Server.Update();
            if (PlayerHandler.Players.Count != 0)
            {
                if (SettingsHandler.DoLevelLock)
                    LevelLockHandler.UpdateCheck();
                foreach (var player in PlayerHandler.Players.Values.Where(x => x.Koala.KoalaName != "SPECTATOR"))
                {
                    if (player.CurrentLevel != player.PreviousLevel && player.PreviousLevel != 99)
                    {
                        HKoala.ReturnKoala(player, player.PreviousLevel);
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
        var connectionType = (ConnectionType)authenticationMessage.GetByte();
        switch (connectionType)
        {
            case ConnectionType.ClientCountRequest:
            {
                Console.WriteLine("Received client count request...");
                var clientCountResponse = Message.Create();
                clientCountResponse.AddByte((byte)ConnectionFailedType.WasClientCountRequest);
                clientCountResponse.AddString("Connection type did not request a login.");
                clientCountResponse.AddInt(_Server.ClientCount);
                _Server.Reject(pendingConnection, clientCountResponse);
                break;
            }
            case ConnectionType.Login:
            {
                Console.WriteLine("Received connection attempt...");
                var pass = authenticationMessage.GetString();
                var spectator = authenticationMessage.GetBool();
                if (!string.Equals(pass, SettingsHandler.ServerSettings.Password, StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(SettingsHandler.ServerSettings.Password, "XXXXX", StringComparison.CurrentCultureIgnoreCase)
                    && !string.IsNullOrWhiteSpace(SettingsHandler.ServerSettings.Password)
                    && _Server.ClientCount > 0)
                {
                    var response = Message.Create();
                    response.AddByte((byte)ConnectionFailedType.IncorrectPassword);
                    response.AddString("The password you entered was incorrect.");
                    _Server.Reject(pendingConnection, response);
                    Console.WriteLine("Rejecting.");
                }
                else if (PlayerHandler.Players.Count(x => x.Value.Koala.KoalaName != "SPECTATOR") == 8 && !spectator)
                {
                    var response = Message.Create();
                    response.AddByte((byte)ConnectionFailedType.ServerFull);
                    response.AddString("The server is full.");
                    _Server.Reject(pendingConnection, response);
                    Console.WriteLine("Rejecting.");
                }
                else
                {
                    _Server.Accept(pendingConnection);
                    Console.WriteLine("Accepting.");
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ClientConnected(object sender, ServerConnectedEventArgs e)
    {
        //Console.WriteLine("Client connected.");
        KoalaHandler.SendKoalaAvailability(e.Client.Id);
        Program.HDrafts.SendTeamData(e.Client.Id);
        SettingsHandler.SendSettings(e.Client.Id);
    }

    private static void ClientDisconnected(object sender, ServerDisconnectedEventArgs e)
    {
        
        if (PlayerHandler.Players.TryGetValue(e.Client.Id, out _))
        {
            PeerMessageHandler.SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Name} has disconnected from the server.", true);
            if (PlayerHandler.Players[e.Client.Id].Koala.KoalaName != "SPECTATOR")
                PeerMessageHandler.SendMessageToClients($"{PlayerHandler.Players[e.Client.Id].Koala.KoalaName} was returned to the koala pool",
                true);
            PlayerHandler.RemovePlayer(e.Client.Id);
            PlayerHandler.AnnounceDisconnect(e.Client.Id);
            Program.HDrafts.TryRemovePlayer(e.Client.Id);
        }
        if (_Server.ClientCount == 0 && SettingsHandler.ServerSettings.ResetPasswordOnEmpty)
            SettingsHandler.ServerSettings.Password = "XXXXX";
    }

    public static void SendCoordinatesToAll(ushort clientId, string koalaName, int level, float[] coordinates, bool onMenu)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.KoalaCoordinates);
        message.AddBool(onMenu);
        message.AddUShort(clientId);
        message.AddString(koalaName);
        message.AddInt(level);
        message.AddFloats(coordinates);
        _Server.SendToAll(message, clientId);
    }
}

public enum ConnectionType : byte
{
    Login,
    ClientCountRequest,
}

public enum ConnectionFailedType : byte
{
    IncorrectPassword,
    Timeout,
    WasClientCountRequest,
    ServerFull
}