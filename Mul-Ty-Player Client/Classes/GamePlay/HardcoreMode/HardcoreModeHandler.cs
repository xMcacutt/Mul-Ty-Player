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
    private HDC_IcicleBehaviour currentIcicleBehaviour;
    
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
        Client.HHero.SetWaterSlideSpeeds(15f, 15f, 30f);
        if (Client.HLevel.CurrentLevelId != Levels.OutbackSafari.Id)
            AttributeHandler.SetBoomerangRange(1200f);
        HardcoreRunDead = false;
        Client.HHero.SetHealth(1);
        Client.HHero.SetWaterHealth(1);
    }
    
    public void InitializeLevel(int level)
    {
        HDC_IcicleHandler.ResetIcicles();
        currentIcicleBehaviour = (HDC_IcicleBehaviour)_random.Next(0, Enum.GetValues(typeof(HDC_IcicleBehaviour)).Length);
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
                ProcessHandler.WriteData(boulderAddr + 0x14C + boulderIndex * 0xA4, BitConverter.GetBytes(3f));
                ProcessHandler.WriteData(boulderAddr + 0x150 + boulderIndex * 0xA4, BitConverter.GetBytes(3f));
            }
        }
        
        ProcessHandler.TryRead(0x25C1F4, out int neddyCount, true, "diveRingCount");
        if (neddyCount != 0)
        {
            ProcessHandler.TryRead(0x25C1F8, out int neddyAddr, true, "diveRingAddress");
            ProcessHandler.WriteData(neddyAddr + 0x60, BitConverter.GetBytes(2.0f));
        }

        var spawnerCount = SpawnerHandler.GetSpawnerCount();
        for (var spawnerIndex = 0; spawnerIndex < spawnerCount; spawnerIndex++)
            SpawnerHandler.MultiplySpawnerDelay(spawnerIndex, 0.5f);
        
        // Set EZoneGate Height
        if (level == Levels.RainbowCliffs.Id)
        {
            var gateYScaleAddr = PointerCalculations.GetPointerAddress(0x269C0C, new[] { 0x12C, 0x0, 0x48, 0x8, 0x58});
            ProcessHandler.WriteData(gateYScaleAddr, BitConverter.GetBytes(1.25f));
        }

        if (level == Levels.CassPass.Id)
        {
            var palmTreeAddr = PointerCalculations.GetPointerAddress(0x274B4C, new[] { 0xC4, 0x70 });
            ProcessHandler.TryRead(palmTreeAddr, out int palmCount, false, "palmCount");
            ProcessHandler.TryRead(palmTreeAddr + 0x4, out palmTreeAddr, false, "palmAddr");
            for (var palmIndex = 0; palmIndex < palmCount; palmIndex++)
            {
                ProcessHandler.WriteData(palmTreeAddr + 0x48 + palmIndex * 0x60, BitConverter.GetBytes(0));
            }
            var staghornAddr = PointerCalculations.GetPointerAddress(0x289954, new[] { 0xEC0, 0x4, 0x2C + 0x44 });
            ProcessHandler.TryRead(staghornAddr, out int staghornCount, false, "staghornCount");
            ProcessHandler.TryRead(staghornAddr + 0x4, out staghornAddr, false, "staghornAddr");
            for (var staghornIndex = 0; staghornIndex < staghornCount; staghornIndex++)
            {
                ProcessHandler.WriteData(staghornAddr + 0x48 + staghornIndex * 0x60, BitConverter.GetBytes(0));
            }
        }
        
        // Reduce TA Time
        ProcessHandler.TryRead(SyncHandler.SaveDataBaseAddress + 0x40 + 0x70 * level, out int currTime, false, "TA Time HC");
        if (currTime == 0)
        {
            ProcessHandler.TryRead(0x28CB58, out int taTime, true, "TA Time");
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x28CB58, BitConverter.GetBytes((int)(taTime * 0.7)));
        }

        Client.HHero.SetRunSpeed(12.5f);
        Client.HHero.SetAirSpeed(12.5f);
        Client.HHero.SetSwimSpeed(25f);
        Client.HHero.SetJumpHeight(19);
        Client.HHero.SetGlideSpeed(10f);
        Client.HHero.SetLedgeGrabTolerance(3f);
        Client.HHero.SetFallDelta(650f);
        Client.HHero.SetOpalMagnetisation(false);
        Client.HHardcore.SetEnemySpeedMultiplier(2.0f);
        Client.HHero.SetWaterSlideSpeeds(20f, 20f, 30f);
        if (Client.HLevel.CurrentLevelId != Levels.OutbackSafari.Id)
            AttributeHandler.SetBoomerangRange(1200f);
        if (Client.HHero.IsBull())
            Client.HHero.SetBushpigSpeeds(50f, 0.6f, 1.5f);
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
        if (HardcoreRunDead && Client.HHero.GetLives() > 0)
        {
            Client.HHero.KillPlayer();
            return;
        }

        var heroState = Client.HHero.GetHeroState();
        var isBull = Client.HHero.IsBull();
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
        if (health < 1 || breath < 1 || (!isBull && heroState == (int)HeroState.KnockedOver))
        {
            EndRun();
            return;
        }

        CheckThEggCounts();
        
        // EMU TIMER
        ProcessHandler.TryRead(0x284AA8, out float emuTimer, true, "emuTimer");
        if (emuTimer > 5000f)
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x284AA8, BitConverter.GetBytes(5000f));
        
        // TURKEY AND BOONIE SPIN
        ProcessHandler.TryRead(0x25D424, out int turkeyCount, true, "getTurkey");
        if (turkeyCount != 0)
        {
            ProcessHandler.TryRead(0x25D428, out int turkeyAddr, true, "getTurkey");
            for (var turkeyIndex = 0; turkeyIndex < turkeyCount; turkeyIndex++)
            {
                var rotationSpeed = _random.Next(0, 2) == 0 ? -0.3f : 0.3f;
                ProcessHandler.WriteData(turkeyAddr + 0x2E8 + turkeyIndex * 0x4A0, BitConverter.GetBytes(rotationSpeed));
            }
        }
        ProcessHandler.TryRead(0x257D5C, out int badBoonieCount, true, "getBadBoonie");
        if (badBoonieCount != 0)
        {
            ProcessHandler.TryRead(0x257D60, out int badBoonieAddr, true, "getBadBoonie");
            for (var badBoonieIndex = 0; badBoonieIndex < badBoonieCount; badBoonieIndex++)
            {
                var rotationSpeed = _random.Next(0, 2) == 0 ? -0.3f : 0.3f;
                ProcessHandler.WriteData(badBoonieAddr + 0x2E8 + badBoonieIndex * 0x4A0, BitConverter.GetBytes(rotationSpeed));
            }
        }

        const float arsonFrillSpawnDelay = 2.5f;
        ProcessHandler.TryRead(0x289D2C, out int arsonFrillSpawnTimer, true, "spawnTimerArson");
        if (arsonFrillSpawnTimer > (int)(arsonFrillSpawnDelay * 60))
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x289D2C, BitConverter.GetBytes((int)(arsonFrillSpawnDelay * 60)));
        
        // Increase Rex Speed
        var rexAddr = PointerCalculations.GetPointerAddress(0x25FF6C, new[] { 0x9C, 0x114 });
        ProcessHandler.TryRead(rexAddr, out float rexSpeed, false, "rexSpeed");
        if (Math.Abs(rexSpeed - 21.125f) < 0.1)
            ProcessHandler.WriteData(rexAddr, BitConverter.GetBytes(21.5f));
        if (Math.Abs(rexSpeed - 9.75) < 0.1)
            ProcessHandler.WriteData(rexAddr, BitConverter.GetBytes(18f));
        if (Math.Abs(rexSpeed - 6.5f) < 0.1)
            ProcessHandler.WriteData(rexAddr, BitConverter.GetBytes(12.5f));


        ProcessHandler.TryRead(0x2704FC, out int taCount, true, "taCount");
        if (taCount != 0)
        {
            var inTimeAttack = Client.HGameState.IsInTimeAttack();
            Client.HGameState.SetMenuItemFlag(TyMenuItem.ExitLevel, TyMenuItemFlag.Enabled, !inTimeAttack);
            Client.HGameState.SetMenuItemFlag(TyMenuItem.MainMenu, TyMenuItemFlag.Enabled, !inTimeAttack);
            ProcessHandler.TryRead(0x270500, out int taAddr, true, "taAddr");
            ProcessHandler.TryRead(taAddr + 0xB4, out int loseDialogAddr, false, "loseDialogAddr");
            ProcessHandler.TryRead(0x28C318, out int lastLoadedDialogAddr, true, "lastDialogAddr");
            if (lastLoadedDialogAddr == loseDialogAddr)
            {
                EndRun();
                return;
            }
        }
        
        // PERIMETER CHECK 
        var addr = PointerCalculations.GetPointerAddress(0x2656F0, new[] { 0x6C });
        ProcessHandler.TryRead(addr, out bool active, false, "Check Perimeter Check");
        if (active && Client.HObjective.GetPerimeterCheckHealth() > 2)
            Client.HObjective.SetPerimeterCheckHealth(2);
        
        // Handle Icicles
        ProcessHandler.TryRead(0x26B964, out int icicleCount, true, "icicleCount");
        if (icicleCount != 0)
        {
            ProcessHandler.TryRead(0x26B968, out int icicleAddr, true, "icicleAddress");
            HDC_IcicleHandler.HandleIcicles(icicleCount, icicleAddr, currentIcicleBehaviour);
        }
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
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25D470, BitConverter.GetBytes(11.50f * (float)Math.Pow(multiplier, 0.5f)));
        //BOONIE
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x257DA8, BitConverter.GetBytes(11.50f * multiplier));
        //WOMBAT
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25F6E8, BitConverter.GetBytes(10.50f * multiplier));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25F6EC, BitConverter.GetBytes(0.02166f * (float)Math.Pow(multiplier, 2)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25F6F0, BitConverter.GetBytes(0.045f * (float)Math.Pow(multiplier, 2)));
        //ANDY
        switch (multiplier)
        {
            case 1:
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25FA28, BitConverter.GetBytes(5f * multiplier));
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25FA30, BitConverter.GetBytes(0.045f * multiplier));
                break;
            case 2:
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25FA28, BitConverter.GetBytes(8.15f * multiplier));
                ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25FA30, BitConverter.GetBytes(0.2f * multiplier));
                break;
        }

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
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C240, BitConverter.GetBytes(20f * (float)Math.Pow(multiplier, 2)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C244, BitConverter.GetBytes(0.02166f * (float)Math.Pow(multiplier, 2)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C2A0, BitConverter.GetBytes(50f * (float)Math.Pow(multiplier, 2)));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x25C2A4, BitConverter.GetBytes(180f * (float)Math.Pow(multiplier, 2)));
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
        if (PlayerHandler.TryGetPlayer(message.GetUShort(), out var player))
            Client.HGameState.DisplayInGameMessage(player.Name + " died... oops!");   
    }

    public void EndRun()
    {
        if (HardcoreRunDead)
            return;
        Client._client.Send(Message.Create(MessageSendMode.Reliable, MessageID.HC_RunStatusChanged).AddBool(true));
        HardcoreRunDead = true;
    }

    private void CheckThEggCounts()
    {
         var count = SyncHandler.HThEg.GlobalObjectData.Values.Take(3).Sum(array => array.Count(x => x == (byte)5));
        if (count >= 17 && SyncHandler.HAttribute.GlobalObjectData[(int)Attributes.GotFlame] == 0)
        {
            var address = SyncHandler.SaveDataBaseAddress + 0xA84;
            ProcessHandler.WriteData(address + (int)RCData.ActivatedBoss1, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)RCData.JuliusMove1, new byte[] { 1 });
            address += 0x20;
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
            ProcessHandler.WriteData(address + (int)Attributes.GotZappy, new byte[] { 1 });
            ProcessHandler.WriteData(address + (int)Attributes.GotTali3, new byte[] { 1 });
            SFXPlayer.PlaySound(SFX.RangGet);
        }
    }
}