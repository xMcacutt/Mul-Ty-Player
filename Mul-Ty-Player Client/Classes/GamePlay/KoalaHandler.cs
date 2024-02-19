﻿using System;
using System.Linq;
using System.Windows.Xps.Serialization;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

internal class KoalaHandler
{
    private static bool readyToWriteTransformData;

    private static readonly int _bTimeAttackAddress = 0x28AB84;
    private static int _baseKoalaAddress;

    public KoalaHandler()
    {
        CreateKoalaAddressArray();
    }

    private static GameStateHandler HGameState => Client.HGameState;
    private static LevelHandler HLevel => Client.HLevel;
    public static KoalaTransformAddresses[] TransformAddresses { get; private set; }

    [MessageHandler((ushort)MessageID.KoalaSelected)]
    private static void HandleKoalaSelected(Message message)
    {
        var koalaName = message.GetString();
        var playerName = message.GetString();
        var clientId = message.GetUShort();
        var isHost = message.GetBool();
        var isReady = message.GetBool();
        var role = (HSRole)message.GetInt();
        var k = Enum.Parse<Koala>(koalaName, true);
        PlayerHandler.AddPlayer(k, playerName, clientId, isHost, isReady, role);
    }

    public void CreateKoalaAddressArray()
    {
        TransformAddresses = new KoalaTransformAddresses[8];
        for (var i = 0; i < 8; i++) TransformAddresses[i] = new KoalaTransformAddresses();
    }

    public void SetBaseAddress()
    {
        _baseKoalaAddress = PointerCalculations.GetPointerAddress(0x26B070, new[] { 0x0 });
        ProcessHandler.CheckAddress(_baseKoalaAddress, (ushort)(17327608 & 0xFFFF), "Koalas base address check");
    }

    public void SetCoordinateAddresses()
    {
        readyToWriteTransformData = false;
        //KOALAS ARE STRUCTURED DIFFERENTLY IN STUMP AND SNOW SO MODIFIER AND OFFSET ARE NECESSARY
        var levelHasKoalas = Levels.GetLevelData(Client.HLevel.CurrentLevelId).HasKoalas;
        var modifier = levelHasKoalas ? 2 : 1;
        var offset = levelHasKoalas ? 0x518 : 0x0;

        if (_baseKoalaAddress == 0)
            SetBaseAddress();

        for (var koalaId = 0; koalaId < 8; koalaId++)
        {
            var koalaOffset = 0x518 * modifier * koalaId + offset;
            TransformAddresses[koalaId] = new KoalaTransformAddresses();
            TransformAddresses[koalaId].X = _baseKoalaAddress + 0x2A4 + koalaOffset;
            TransformAddresses[koalaId].Y = _baseKoalaAddress + 0x2A8 + koalaOffset;
            TransformAddresses[koalaId].Z = _baseKoalaAddress + 0x2AC + koalaOffset;
            TransformAddresses[koalaId].Pitch = _baseKoalaAddress + 0x2B4 + koalaOffset;
            TransformAddresses[koalaId].Yaw = _baseKoalaAddress + 0x2B8 + koalaOffset;
            TransformAddresses[koalaId].Roll = _baseKoalaAddress + 0x2BC + koalaOffset;
            TransformAddresses[koalaId].Collision = _baseKoalaAddress + 0x298 + koalaOffset;
            TransformAddresses[koalaId].Visibility = _baseKoalaAddress + 0x44 + koalaOffset;
            TransformAddresses[koalaId].Scale = _baseKoalaAddress + 0x60 + koalaOffset;
            TransformAddresses[koalaId].State = _baseKoalaAddress + 0x98 + koalaOffset;
        }
        
        ScaleKoalas();
        SetKoalaState();
        MakeVisible();

        if (!SettingsHandler.Settings.DoKoalaCollision)
            SetCollision();

        readyToWriteTransformData = true;
    }

    public void SetKoalaState()
    {
        // Turns koala state to 0 to avoid audio playing from koalas
        var value = SettingsHandler.DoHideSeek ? (byte)0 : (byte)1;
        for (var i = 0; i < 8; i++)
            ProcessHandler.WriteData(TransformAddresses[i].State, new byte[] { value }, "Removing voice lines");
    }

    public void SetCollision()
    {
        var col = SettingsHandler.Settings.DoKoalaCollision ? (byte)1 : (byte)0;
        for (var i = 0; i < 8; i++)
            ProcessHandler.WriteData(TransformAddresses[i].Collision, new byte[] { col }, "Removing collision");
    }

    public void CheckTA()
    {
        ProcessHandler.TryRead(_bTimeAttackAddress, out int inTimeAttack, true, "KoalaHandler::CheckTA()");
        if (inTimeAttack == 1)
            MakeVisible();
    }

    public void MakeVisible()
    {
        for (var i = 0; i < 8; i++)
            ProcessHandler.WriteData(TransformAddresses[i].Visibility, new byte[] { 1 }, "Making players visible");
    }

    public void ScaleKoalas()
    {
        var outbackMultiplier = 1.0f;
        if (HLevel.CurrentLevelId == 10) outbackMultiplier = 2.0f;
        var initialScale = SettingsHandler.DoHideSeek ? 2.25f : SettingsHandler.Settings.KoalaScale;
        var scaleFactor = initialScale * outbackMultiplier;
        if (scaleFactor < 0.5f) scaleFactor = 0.5f;
        if (scaleFactor > 3) scaleFactor = 3;
        const float snugsMultiplier = 1.0725f;
        const float katieMultiplier = 0.825f;
        for (var koalaIndex = 0; koalaIndex < 8; koalaIndex++)
        {
            var tempScaleFactor = scaleFactor;
            if (koalaIndex == 0) tempScaleFactor = scaleFactor * katieMultiplier;
            if (koalaIndex == 3) tempScaleFactor = scaleFactor * snugsMultiplier;
            ProcessHandler.WriteData(TransformAddresses[koalaIndex].Scale, BitConverter.GetBytes(tempScaleFactor),
                "Scaling koalas");
        }
    }

    [MessageHandler((ushort)MessageID.KoalaCoordinates)]
    private static void HandleGettingCoordinates(Message message)
    {
        //If this client isnt in game, or hasnt selected a koala, return
        if (!Client.KoalaSelected || Client.Relaunching)
            return;
        //Debug.WriteLine("Received koala coord");
        var onMenu = message.GetBool();
        var clientId = message.GetUShort();
        var koalaName = message.GetString();
        var k = (Koala)Enum.Parse(typeof(Koala), koalaName, true);
        var koalaId = (int)k;
        var level = message.GetInt();

        //Set the incoming players current level code
        if (!PlayerHandler.TryGetPlayer(clientId, out var player))
            return;
        player.Level = onMenu ? "M/L" : Levels.GetLevelData(level).Code;

        //Return if player is on the main menu or loading screen,
        //No need to set coords
        if (HGameState.IsOnMainMenuOrLoading)
        {
            readyToWriteTransformData = false;
            return;
        }

        //If failed to get this clients player, or received our own coordinates, return
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self) || Koalas.GetInfo[self.Koala].Name == koalaName)
            return;

        //If the received player has finished the game, return
        if (level == Levels.EndGame.Id)
            return;

        var transform = message.GetFloats();
        //Debug.WriteLine($"Handle coordinates: {KoalaTransform.DebugTransform(transform)}");
        //Debug.WriteLine($"Before updating coordinates: {KoalaTransform.DebugTransform(playerTransformAddresses[koalaID].New.Transform)}");
        PlayerReplication.UpdatePlayerSnapshotData(koalaId, transform, level);
        //Debug.WriteLine($"After updating coordinates: {KoalaTransform.DebugTransform(playerTransformAddresses[koalaID].New.Transform)}");
    }

    public struct KoalaTransformAddresses
    {
        public int X, Y, Z, Pitch, Yaw, Roll, Collision, Visibility, Scale, State;
    }
}