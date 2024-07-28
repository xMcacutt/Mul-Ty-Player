using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Classes.Views;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient;

public class HeroHandler
{
    private const int TY_POSITION_ADDRESS = 0x270B78;
    private const int TY_ROTATION_ADDRESS = 0x271C1C;

    private const int BP_POSITION_ADDRESS = 0x254268;
    private const int BP_ROTATION_ADDRESS = 0x2545F0;

    private readonly float[] currentPositionRotation;

    private int positionAddress = TY_POSITION_ADDRESS;
    private int rotationAddress = TY_ROTATION_ADDRESS;

    private float[] heldPosition = new [] { 0f, 0f, 0f };

    private Dictionary<int, float> _deathPlaneHeight = new Dictionary<int, float>()
    {
        {0, -2500},
        {4, -3000},
        {5, -6000},
        {6, -4000},
        {7, -2000},
        {8, -4000},
        {9, -13500},
        {12, -6500},
        {13, -10000},
        {14, -7000},
        {15, -1500},
        {17, -4500},
        {19, -6000},
        {20, -3500},
        {21, -2000},
        {22, -2000},
        {23, -3000},
    };

    public HeroHandler()
    {
        currentPositionRotation = new float[6];
        HLevel.OnLevelChange += CheckOutbackSafari;
    }

    private static LevelHandler HLevel => Client.HLevel;
    private static GameStateHandler HGameState => Client.HGameState;

    public void GetTyPosRot()
    {
        for (var i = 0; i < 3; i++)
        {
            ProcessHandler.TryRead(positionAddress + sizeof(float) * i, out currentPositionRotation[i], true,
                "HeroHandler::GetTyPosRot() {position}");
            ProcessHandler.TryRead(rotationAddress + sizeof(float) * i, out currentPositionRotation[i + 3], true,
                "HeroHandler::GetTyPosRot() {rotation}");
        }
        CheckDeathPlane();
    }

    private void CheckDeathPlane()
    {
        if (_deathPlaneHeight.ContainsKey(HLevel.CurrentLevelId))
            if (currentPositionRotation[1] < _deathPlaneHeight[HLevel.CurrentLevelId])
                KillPlayer();
    }

    public float[] GetCurrentPosRot()
    {
        return currentPositionRotation;
    }

    public void SetSwimSpeed(float speed = 20f)
    {
        ProcessHandler.UnprotectMemory<float>((int)TyProcess.BaseAddress + 0x1F982C);
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x1F982C, BitConverter.GetBytes(speed));
    }
    
    public void SetHealth(int value)
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2737CC, BitConverter.GetBytes(value));
    }
    
    public int GetHealth()
    {
        ProcessHandler.TryRead(0x2737CC, out int health, true, "GetHealth()");
        return health;
    }
    
    public void SetWaterHealth(int value)
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2737F0, BitConverter.GetBytes(value));
    }
    
    public int GetWaterHealth()
    {
        ProcessHandler.TryRead(0x2737F0, out int health, true, "GetHealth()");
        return health;
    }
    
    public void SetOutbackHealth(int value)
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x273814, BitConverter.GetBytes(value));
    }
    
    public int GetOutbackHealth()
    {
        ProcessHandler.TryRead(0x273814, out int health, true, "GetHealth()");
        return health;
    }

    public void SetHeroState(int state)
    {
        if (Client.HLevel.CurrentLevelData.Id == Levels.OutbackSafari.Id)
        {
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x254560, BitConverter.GetBytes(state));
        }
        else
        {
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27158C, BitConverter.GetBytes(state));
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x26EE4C, BitConverter.GetBytes(state));
        }
    }
    
    public void WritePosition(float x, float y, float z, bool log = true)
    {
        var bytes = BitConverter.GetBytes(x)
            .Concat(BitConverter.GetBytes(y))
            .Concat(BitConverter.GetBytes(z))
            .ToArray();
        if (log)
            Logger.Write($"Teleported to {x}, {y}, {z}");
        heldPosition = new[] { x, y, z };
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + positionAddress, bytes);
    }

    private void CheckOutbackSafari(int levelId)
    {
        if (levelId == Levels.OutbackSafari.Id)
        {
            positionAddress = BP_POSITION_ADDRESS;
            rotationAddress = BP_ROTATION_ADDRESS;
        }
        else
        {
            positionAddress = TY_POSITION_ADDRESS;
            rotationAddress = TY_ROTATION_ADDRESS;
        }
    }

    public void SendCoordinates()
    {
        //Debug.WriteLine("SENDING KOALA COORDS");
        //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
        message.AddBool(HGameState.IsAtMainMenu());
        message.AddInt(HLevel.CurrentLevelId);
        message.AddFloats(Client.HHero.currentPositionRotation);
        Client._client.Send(message);
    }

    public void SetRunSpeed(float speed = 10.0f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288914), BitConverter.GetBytes(speed));
    }

    public void SetGravity(float gravity = 0.75f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288940), BitConverter.GetBytes(gravity));
    }
    
    public void SetAirSpeed(float airSpeed = 10f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288920), BitConverter.GetBytes(airSpeed));
    }
    
    public void SetDoubleJumpHeight(float doubleJumpHeight = 8.37f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x28894C), BitConverter.GetBytes(doubleJumpHeight));
    }

    public void SetJumpHeight(float jumpHeight = 18.57f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x28893C), BitConverter.GetBytes(jumpHeight));
    }
    
    public void SetLedgeGrabTolerance(float tolerance = 20f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288968), BitConverter.GetBytes(tolerance));
    }

    public void SetDefaults()
    {
        Client.HHero.SetSwimSpeed();
        Client.HHero.SetRunSpeed();
        Client.HHero.SetGravity();
        Client.HHero.SetGlideSpeed();
        Client.HHero.SetAirSpeed();
        Client.HHero.SetJumpHeight();
        Client.HHero.SetLedgeGrabTolerance();
        Client.HHero.SetDoubleJumpHeight();
    }
    
    public void SetGlideSpeed(float speed = 7f)
    {
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288928), BitConverter.GetBytes(speed));
    }
    
    public void ToggleInvincibility()
    {
        ProcessHandler.TryRead(0x288A55, out bool result, true, "ToggleInvincibility()");
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288A55), BitConverter.GetBytes(!result));
    }

    public void GiveTechnorangs()
    {
        var saveAddress = PointerCalculations.GetPointerAddress(0x288730, new int[1]);
        ProcessHandler.WriteData(saveAddress + 0xAB6, new byte[] { 1 });
        ProcessHandler.WriteData(saveAddress + 0xABB, new byte[] { 1, 1, 1, 1, 1 });
        ProcessHandler.WriteData(saveAddress + 0xAC2, new byte[] { 1, 1 });
    }
    
    public void ToggleElementals()
    {
        var saveAddress = PointerCalculations.GetPointerAddress(0x288730, new int[1]);
        ProcessHandler.WriteData(saveAddress + 0xAB4, new byte[] { 1, 1, 1 });
        ProcessHandler.WriteData(saveAddress + 0xAB9, new byte[] { 1, 1 });
        ProcessHandler.WriteData(saveAddress + 0xAC0, new byte[] { 1, 1 });
    }
    
    public void WriteHeldPosition()
    {
        var bytes = BitConverter.GetBytes(heldPosition[0])
            .Concat(BitConverter.GetBytes(heldPosition[1]))
            .Concat(BitConverter.GetBytes(heldPosition[2]))
            .ToArray();
        Logger.Write($"Teleported to {heldPosition[0]}, {heldPosition[1]}, {heldPosition[2]}");
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + positionAddress, bytes);
    }

    public void ChangeSkin(int index)
    {
        //["Mul-Ty-Player.exe" + 0x289298] +0x5E8;
        var address = PointerCalculations.GetPointerAddress(0x289298, new[] { 0x5E8 });
        ProcessHandler.WriteData(address, BitConverter.GetBytes(address - 68 - (76 * index)));
    }
    
    public void SetTyInvis(bool isInvis)
    {
        var address = PointerCalculations.GetPointerAddress(0x289298, new[] { 0x5E8 });
        ProcessHandler.WriteData(address, BitConverter.GetBytes(address - 68 - (76 * 10)));
        address -= 0x2AC;
        ProcessHandler.WriteData(address, BitConverter.GetBytes(address - 68 - (76 * 10)));
    }

    public void KillPlayer()
    {
        if (HLevel.CurrentLevelId == 10)
            SetHeroState(7);
        else
            SetHeroState(29);
    }

    public int GetHeroState()
    {
        int result;
        if (Client.HLevel.CurrentLevelData.Id == Levels.OutbackSafari.Id)
            ProcessHandler.TryRead(0x254560, out result, true, "Get Hero State Outback");
        else
            ProcessHandler.TryRead(0x27158C, out result, true, "Get Hero State Ty");
        return result;
    }

    [MessageHandler((ushort)MessageID.Kill)]
    public static void HandleKillReceive(Message message)
    {
        Client.HHero.KillPlayer();
    }
}