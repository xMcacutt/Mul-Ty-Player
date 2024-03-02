using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using MulTyPlayer;
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
    }

    public float[] GetCurrentPosRot()
    {
        return currentPositionRotation;
    }

    public void SetHealth(int value)
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2737CC, BitConverter.GetBytes(value));
    }

    public void SetHeroState(int state)
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27158C, BitConverter.GetBytes(state));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x26EE4C, BitConverter.GetBytes(state));
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

    public void CheckOutbackSafari(int levelId)
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
}