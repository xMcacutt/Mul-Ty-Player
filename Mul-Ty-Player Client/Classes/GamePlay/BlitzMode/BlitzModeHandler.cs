using System;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient.Classes.GamePlay.BlitzMode;

public class BlitzModeHandler
{
    private Random _random = new Random();
    
    public void Initialize()
    {
        _random = new Random();
        Client.HHero.SetRunSpeed(12.5f);
        Client.HHero.SetAirSpeed(12.5f);
        Client.HHero.SetSwimSpeed(25f);
        Client.HHero.SetJumpHeight(19);
        Client.HHero.SetGlideSpeed(10f);
        Client.HHero.SetFallDelta(3000f);
        Client.HHero.SetOpalMagnetisation(true);
        Client.HBlitz.SetEnemySpeedMultiplier(2.0f);
        Client.HHero.SetWaterSlideSpeeds(10f, 20f, 30f);
        if (Client.HLevel.CurrentLevelId != Levels.OutbackSafari.Id)
            AttributeHandler.SetBoomerangRange(1600f);
        Client.HHero.SetHealth(4);
        Client.HHero.SetWaterHealth(4);
    }
    
    public void InitializeLevel(int level)
    {
        SyncHandler.HThEg.ShowStopwatch();
        LiveMushroomSyncer.SetMushroomState(true);

        Client.HHero.SetRunSpeed(12.5f);
        Client.HHero.SetAirSpeed(12.5f);
        Client.HHero.SetSwimSpeed(25f);
        Client.HHero.SetJumpHeight(19);
        Client.HHero.SetGlideSpeed(10f);
        Client.HHero.SetFallDelta(3000f);
        Client.HHero.SetOpalMagnetisation(true);
        Client.HBlitz.SetEnemySpeedMultiplier(2.0f);
        Client.HHero.SetWaterSlideSpeeds(10f, 20f, 30f);
        if (Client.HLevel.CurrentLevelId != Levels.OutbackSafari.Id)
            AttributeHandler.SetBoomerangRange(1600f);
        Client.HHero.SetHealth(4);
        Client.HHero.SetWaterHealth(4);
    }

    public void Deinitialize()
    {
        Client.HHero.SetDefaults();
    }

    public void Run()
    {
        if (Client.HHero.GetLives() < 1)
            Client.HHero.SetLives(1);
        // Read Breath
        var breath = Client.HHero.GetWaterHealth();
        // Set Breath
        if (breath < 8)
            Client.HHero.SetWaterHealth(8);
        // Read Health
        var health = Client.HHero.GetHealth();
        // Set Health
        if (health < 4)
            Client.HHero.SetHealth(4);
        // THEGG RANGS
        Client.HHardcore.CheckThEggCounts();
    }

    public void SetEnemySpeedMultiplier(float multiplier = 1)
    {
        //TURKEY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25D470,
            BitConverter.GetBytes(11.50f * (1f / multiplier)));
        //BOONIE
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x257DA8,
            BitConverter.GetBytes(11.50f * (1f / multiplier)));
        // CABLE CAR
        ProcessHandler.UnprotectMemory<float>((int)TyProcess.BaseAddress + 0x1FA248);
        ProcessHandler.UnprotectMemory<float>((int)TyProcess.BaseAddress + 0x205924);
        switch (multiplier)
        {
            case 1:
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x205924, BitConverter.GetBytes(0.07f));
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x1FA248, BitConverter.GetBytes(180f));
                break;
            case 2:
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x205924,
                    BitConverter.GetBytes(0.25f * multiplier));
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x1FA248,
                    BitConverter.GetBytes(300f * multiplier));
                break;
        }

        //ARSONFRILL
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25AE48,
            BitConverter.GetBytes(5.0f * (1f / multiplier)));
        //SLY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25E438,
            BitConverter.GetBytes(4.0f * (1f / multiplier)));
        //NEDDY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C23C,
            BitConverter.GetBytes(1.60f * (1f / multiplier)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C240, BitConverter.GetBytes(20f * (1f / multiplier)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C244,
            BitConverter.GetBytes(0.02166f * (1f / multiplier)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C2A0, BitConverter.GetBytes(50f * (1f / multiplier)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C2A4,
            BitConverter.GetBytes(180f * (1f / multiplier)));
        //DENNIS
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x259A98, BitConverter.GetBytes(6.0f * multiplier));
        //CHEMICALFRILL
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25ACAC,
            BitConverter.GetBytes(2.0f * (1f / multiplier)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25ACB4,
            BitConverter.GetBytes(0.032724f * (1f / multiplier)));
    }
}