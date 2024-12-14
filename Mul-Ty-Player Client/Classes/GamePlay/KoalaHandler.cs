using System;
using System.Linq;
using System.Windows.Xps.Serialization;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

internal class KoalaHandler
{
    private static bool readyToWriteTransformData;

    private static readonly int _bTimeAttackAddress = 0x28CA3C;
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
        var score = message.GetInt();
        var vip = (VIP)message.GetInt();
        Koala? k;
        if (role == HSRole.Spectator || koalaName == "SPECTATOR") // enum check faster than string comp, but leave strcmp incase i forgor something
            k = null;
        else
            k = Enum.Parse<Koala>(koalaName, true);
        PlayerHandler.AddPlayer(k, playerName, clientId, isHost, isReady, role, score, vip, Client.KoalaSelected);
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
            TransformAddresses[koalaId] = new KoalaTransformAddresses
            {
                X = _baseKoalaAddress + 0x2A4 + koalaOffset,
                Y = _baseKoalaAddress + 0x2A8 + koalaOffset,
                Z = _baseKoalaAddress + 0x2AC + koalaOffset,
                Pitch = _baseKoalaAddress + 0x2B4 + koalaOffset,
                Yaw = _baseKoalaAddress + 0x2B8 + koalaOffset,
                Roll = _baseKoalaAddress + 0x2BC + koalaOffset,
                Collision = _baseKoalaAddress + 0x298 + koalaOffset,
                Visibility = _baseKoalaAddress + 0x44 + koalaOffset,
                Scale = _baseKoalaAddress + 0x60 + koalaOffset
            };
        }
        
        ScaleKoalas();
        MakeVisible();

        if (!SettingsHandler.ClientSettings.DoKoalaCollision)
            SetCollision();

        readyToWriteTransformData = true;
    }
    
    public void SetCollision()
    {
        var col = SettingsHandler.ClientSettings.DoKoalaCollision ? (byte)1 : (byte)0;
        for (var i = 0; i < 8; i++)
            ProcessHandler.WriteData(
                TransformAddresses[i].Collision, 
                new byte[] { col }, 
                "Removing collision");
    }

    private bool wasInTimeAttack;
    public void HandleTA()
    {
        var inTimeAttack = Client.HGameState.IsInTimeAttack();
        if (wasInTimeAttack && inTimeAttack)
        {
            SetCollision();
            wasInTimeAttack = false;
        }
        if (!inTimeAttack) 
            return;
        MakeVisible();
        if (SettingsHandler.ClientSettings.ShowCollectiblesInTA)
            SyncHandler.MakeCollectiblesVisibleInTimeAttack();
        wasInTimeAttack = true;
    }

    public void MakeVisible()
    {
        for (var i = 0; i < 8; i++)
            ProcessHandler.WriteData(TransformAddresses[i].Visibility, 
                new byte[] { 1 }, 
                "Making players visible");
    }

    public void ScaleKoalas()
    {
        var outbackMultiplier = 1.0f;
        if (HLevel.CurrentLevelId == 10) outbackMultiplier = 2.0f;
        var initialScale = SettingsHandler.GameMode == GameMode.HideSeek ? 2.25f : SettingsHandler.ClientSettings.KoalaScale;
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
    
    public void ScaleKoalas(float size)
    {
        var outbackMultiplier = 1.0f;
        if (HLevel.CurrentLevelId == 10) outbackMultiplier = 2.0f;
        var initialScale = size;
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
        if (!ModelController.Login.JoinAsSpectator)
            if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self) || Koalas.GetInfo[(Koala)self.Koala].Name == koalaName)
                return;

        //If the received player has finished the game, return
        if (level == Levels.EndGame.Id)
            return;

        var transform = message.GetFloats();
        PlayerReplication.UpdatePlayerSnapshotData(koalaId, transform, onMenu, level);
    }

    public struct KoalaTransformAddresses
    {
        public int X, Y, Z, Pitch, Yaw, Roll, Collision, Visibility, Scale;
    }
}