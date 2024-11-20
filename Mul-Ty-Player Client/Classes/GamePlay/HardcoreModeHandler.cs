using System;
using System.Linq;
using MulTyPlayer;
using MulTyPlayerClient.GUI;
using Riptide;

namespace MulTyPlayerClient.Classes.GamePlay;

public class HardcoreModeHandler
{
    public bool HardcoreRunDead;
    private Random _random = new Random();
    
    public void Initialize()
    {
        _random = new Random();
        Client.HHero.SetRunSpeed(12.5f);
        Client.HHero.SetAirSpeed(12.5f);
        Client.HHero.SetSwimSpeed(25f);
        Client.HHero.SetJumpHeight(19);
        Client.HHero.SetGlideSpeed(10f);
        Client.HHero.SetLedgeGrabTolerance(3f);
        Client.HHero.SetFallDelta(650f);
        Client.HHero.SetOpalMagnetisation(false);
        Client.HHardcore.SetEnemySpeedMultiplier(2.0f);
        AttributeHandler.SetBoomerangRange(1200f);
    }
    
    public void InitializeLevel(int level)
    {
        SyncHandler.HThEg.ShowStopwatch();
        var triggerSphereCount = Client.HSync.HTrigger.GetTriggerSphereCount();
        for (var triggerIndex = 0; triggerIndex < triggerSphereCount; triggerIndex++)
        {
            var triggerEntryAddr = Client.HSync.HTrigger.GetTriggerEnterTargetAddress(triggerIndex);
            var triggerExitAddr = Client.HSync.HTrigger.GetTriggerExitTargetAddress(triggerIndex);
            if (triggerEntryAddr != triggerExitAddr)
                continue;
            ProcessHandler.TryRead(triggerEntryAddr + 4, out int indicatorAddr, false, "Find Teleporter Trigger");
            if (indicatorAddr != TyProcess.BaseAddress + 0x26D68C)
                continue;
            Client.HSync.HTrigger.SetTriggerActivity(triggerIndex, true);
            ProcessHandler.WriteData(triggerEntryAddr + 0xC8, new byte[] { 0x0, 0x1, 0x1 });
        }

        ProcessHandler.TryRead(0x2665AC, out int diveRingCount, true, "diveRingCount");
        if (diveRingCount != 0)
        {
            var averageDiveRingX = 0f;
            var averageDiveRingZ = 0f;
            ProcessHandler.TryRead(0x2665B0, out int diveRingAddr, true, "diveRingAddress");
            for (var diveRingIndex = 0; diveRingIndex < diveRingCount; diveRingIndex++)
            {
                ProcessHandler.WriteData(diveRingAddr + 0x4C + diveRingIndex * 0xA4, BitConverter.GetBytes(2f));
                ProcessHandler.WriteData(diveRingAddr + 0x64 + diveRingIndex * 0xA4, BitConverter.GetBytes(2f));
                ProcessHandler.WriteData(diveRingAddr + 0x64 + 0x18 + diveRingIndex * 0xA4, BitConverter.GetBytes(-2f));
                ProcessHandler.WriteData(diveRingAddr + 0x64 + 0x18 + 0xC + diveRingIndex * 0xA4, BitConverter.GetBytes(2f));
                ProcessHandler.TryRead(diveRingAddr + 0x54 + diveRingIndex * 0xA4, out float diveRingX, false, "diveRingX");
                ProcessHandler.TryRead(diveRingAddr + 0x5C + diveRingIndex * 0xA4, out float diveRingZ, false, "diveRingX");
                averageDiveRingX += diveRingX;
                averageDiveRingZ += diveRingZ;
            }

            averageDiveRingX /= diveRingCount;
            averageDiveRingZ /= diveRingCount;
            
            for (var diveRingIndex = 0; diveRingIndex < diveRingCount; diveRingIndex++)
            {
                var diveRingX = 0f;
                var diveRingZ = 0f;
                var diveRingRange = 450f;
                diveRingX = _random.Next(0, 2) == 1
                    ? averageDiveRingX + _random.NextSingle() * diveRingRange
                    : averageDiveRingX - _random.NextSingle() * diveRingRange;
                diveRingZ = _random.Next(0, 2) == 1
                    ? averageDiveRingZ + _random.NextSingle() * diveRingRange
                    : averageDiveRingZ - _random.NextSingle() * diveRingRange;
                ProcessHandler.WriteData(diveRingAddr + 0x54 + diveRingIndex * 0xA4, BitConverter.GetBytes(diveRingX));
                ProcessHandler.WriteData(diveRingAddr + 0x5C + diveRingIndex * 0xA4, BitConverter.GetBytes(diveRingZ));
                ProcessHandler.WriteData(diveRingAddr + 0x94 + diveRingIndex * 0xA4, BitConverter.GetBytes(diveRingX));
                ProcessHandler.WriteData(diveRingAddr + 0x9C + diveRingIndex * 0xA4, BitConverter.GetBytes(diveRingZ));
            }
        }

        ProcessHandler.TryRead(0x253A14, out int bouncingBoulderCount, true, "diveRingCount");
        if (bouncingBoulderCount != 0)
        {
            ProcessHandler.TryRead(0x253A18, out int boulderAddr, true, "diveRingAddress");
            for (var boulderIndex = 0; boulderIndex < bouncingBoulderCount; boulderIndex++)
            {
                ProcessHandler.WriteData(boulderAddr + 0x14C + boulderIndex * 0xA4, BitConverter.GetBytes(2f));
                ProcessHandler.WriteData(boulderAddr + 0x150 + boulderIndex * 0xA4, BitConverter.GetBytes(2f));
            }
        }
        
        ProcessHandler.TryRead(0x253AC4, out int rollingBoulderCount, true, "diveRingCount");
        if (rollingBoulderCount != 0)
        {
            ProcessHandler.TryRead(0x253AC8, out int boulderAddr, true, "diveRingAddress");
            for (var boulderIndex = 0; boulderIndex < rollingBoulderCount; boulderIndex++)
            {
                ProcessHandler.WriteData(boulderAddr + 0x14C + boulderIndex * 0xA4, BitConverter.GetBytes(2f));
                ProcessHandler.WriteData(boulderAddr + 0x150 + boulderIndex * 0xA4, BitConverter.GetBytes(2f));
            }
        }

        var spawnerCount = SpawnerHandler.GetSpawnerCount();
        for (var spawnerIndex = 0; spawnerIndex < spawnerCount; spawnerIndex++)
            SpawnerHandler.MultiplySpawnerDelay(spawnerIndex, 0.5f);
        
        Client.HHero.SetRunSpeed(12.5f);
        Client.HHero.SetAirSpeed(12.5f);
        Client.HHero.SetSwimSpeed(25f);
        Client.HHero.SetJumpHeight(19);
        Client.HHero.SetGlideSpeed(10f);
        Client.HHero.SetLedgeGrabTolerance(3f);
        Client.HHero.SetFallDelta(650f);
        Client.HHero.SetOpalMagnetisation(false);
        Client.HHardcore.SetEnemySpeedMultiplier(2.0f);
        AttributeHandler.SetBoomerangRange(1200f);
    }

    public void Deinitialize()
    {
        Client.HHero.SetDefaults();
    }

    public void Run()
    {
        // Set Hardcore Mode
        ProcessHandler.WriteData(SyncHandler.SaveDataBaseAddress - 0x2, new byte[] { 0x1 });

        if (Client.HHero.GetLives() > 1)
            Client.HHero.SetLives(1);
        
        if (Client.HHero.GetLives() < 0)
            Client.HHero.SetLives(0);
        
        // Check Dead Run
        if (HardcoreRunDead && Client.HHero.GetLives() >= 1)
        {
            Client.HHero.KillPlayer();
            return;
        }
        
        // Read Breath
        var breath = Client.HHero.GetWaterHealth();
        // Set Breath
        if (breath > 2)
            Client.HHero.SetWaterHealth(2);
        // Read Health
        var health = Client.HHero.GetHealth();
        // Set Health
        if (health > 1)
            Client.HHero.SetHealth(1);
        // Check Dead
        if (health < 1)
        {
            Client._client.Send(Message.Create(MessageSendMode.Reliable, MessageID.HC_RunStatusChanged).AddBool(true));
            HardcoreRunDead = true;
            return;
        }

        var count = SyncHandler.HThEg.GlobalObjectData.Values.Take(3).Sum(array => array.Count(x => x == (byte)5));
        if (count >= 17 && SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotFlame] == 0)
        {
            var address = SyncHandler.SaveDataBaseAddress + 0xA84;
            ProcessHandler.WriteData(address + (int)RCData.ActivatedBoss1, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)RCData.JuliusMove1, new byte[] { 1 });
            address += 0x20;
            SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotFlame] = 1;
            SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotTali1] = 1;
            ProcessHandler.WriteData(address + (int)Attributes.GotFlame, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)Attributes.GotTali1, new byte[] { 1 });
            SFXPlayer.PlaySound(SFX.RangGet);
        }
        
        count = SyncHandler.HThEg.GlobalObjectData.Values.Skip(3).Take(3).Sum(array => array.Count(x => x == (byte)5));
        if (count >= 17 && SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotFrosty] == 0)
        {
            var address = SyncHandler.SaveDataBaseAddress + 0xA84;
            ProcessHandler.WriteData(address + (int)RCData.ActivatedBoss2, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)RCData.JuliusMove2, new byte[] { 1 });
            address += 0x20;
            SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotFrosty] = 1;
            SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotTali2] = 1;
            ProcessHandler.WriteData(address + (int)Attributes.GotFrosty, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)Attributes.GotTali2, new byte[] { 1 });
            SFXPlayer.PlaySound(SFX.RangGet);
        }
        
        count = SyncHandler.HThEg.GlobalObjectData.Values.Skip(6).Take(3).Sum(array => array.Count(x => x == (byte)5));
        if (count >= 17 && SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotZappy] == 0)
        {
            var address = SyncHandler.SaveDataBaseAddress + 0xA84;
            ProcessHandler.WriteData(address + (int)RCData.ActivatedBoss3, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)RCData.JuliusMove3, new byte[] { 1 });
            address += 0x20;
            SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotZappy] = 1;
            SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotTali3] = 1;
            ProcessHandler.WriteData(address + (int)Attributes.GotZappy, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)Attributes.GotTali3, new byte[] { 1 });
            SFXPlayer.PlaySound(SFX.RangGet);
        }
        
        ProcessHandler.TryRead(0x25D428, out int turkeyAddr, true, "getTurkey");
        var rotationSpeed = _random.Next(0, 2) == 0 ? -0.3f : 0.3f;
        if (turkeyAddr != 0)
            ProcessHandler.WriteData(turkeyAddr + 0x2E8, BitConverter.GetBytes(rotationSpeed));
        
        // PERIMETER CHECK 
        var addr = PointerCalculations.GetPointerAddress(0x2656F0, new[] { 0x6C });
        ProcessHandler.TryRead(addr, out bool active, false, "Check Perimeter Check");
        if (active && Client.HObjective.GetPerimeterCheckHealth() > 2)
            Client.HObjective.SetPerimeterCheckHealth(2);
    }

    public void SetEnemySpeedMultiplier(float multiplier = 1)
    {
        //FRILL
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25AB18, BitConverter.GetBytes(5f * multiplier));
        //RUFUS
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25D0F8, BitConverter.GetBytes(7.0f * multiplier));
        //CRAB
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25BFD0, BitConverter.GetBytes(5.33f * multiplier));
        //MUDCRAB
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25BDF0, BitConverter.GetBytes(6.0f * multiplier));
        //BLUETONGUE
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2584AC, BitConverter.GetBytes(1.2f * multiplier));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2584B0, BitConverter.GetBytes(4.3f * multiplier));
        //SKINK
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25E000, BitConverter.GetBytes(4.3f * multiplier));
        //SHARK
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25DD60, BitConverter.GetBytes(6.666f * multiplier));
        //BIKERFRILL
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25A898, BitConverter.GetBytes(float.Min(32.50f * multiplier, 50f)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25A89C, BitConverter.GetBytes(0.0192f * (float)Math.Pow(multiplier, 1)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25A8A0, BitConverter.GetBytes(0.0997475f * (float)Math.Pow(multiplier, 1)));
        //TURKEY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25D470, BitConverter.GetBytes(11.50f * multiplier));
        //BOONIE
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x257DA8, BitConverter.GetBytes(11.50f * multiplier));
        //WOMBAT
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25F6E8, BitConverter.GetBytes(10.50f * multiplier));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25F6EC, BitConverter.GetBytes(0.02166f * (float)Math.Pow(multiplier, 2)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25F6F0, BitConverter.GetBytes(0.045f * (float)Math.Pow(multiplier, 2)));
        //ARSONFRILL
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25AE48, BitConverter.GetBytes(5.0f * multiplier));
        //LILNEDDY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25BB60, BitConverter.GetBytes(4.7f * multiplier));
        //SLY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25E438, BitConverter.GetBytes(4.0f * multiplier));
        //BARRACUDA
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x258010, BitConverter.GetBytes(8.00f * multiplier));
        //NEDDY
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C23C, BitConverter.GetBytes(1.60f * multiplier));
        //DENNIS
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x259A98, BitConverter.GetBytes(6.0f * multiplier));
        //CHEMICALFRILL
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25ACAC, BitConverter.GetBytes(2.0f * multiplier));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25ACB4, BitConverter.GetBytes(0.032724f * (float)Math.Pow(multiplier, 2.5f)));
        //RHINO
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25CBBC, BitConverter.GetBytes(3.30f * multiplier));
        //CROC
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2597B8, BitConverter.GetBytes(5.50f * multiplier));
        //NINJA
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C4F0, BitConverter.GetBytes(10.40f * multiplier));
    }
    
    [MessageHandler((ushort)MessageID.HC_RunStatusChanged)]
    public static void HandleRunStatusChanged(Message message)
    {
        Client.HHardcore.HardcoreRunDead = message.GetBool();
    }
}