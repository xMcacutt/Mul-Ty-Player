using System;
using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;

namespace MulTyPlayerClient;

internal class LiveMushroomSyncer : LiveDataSyncer
{
    public LiveMushroomSyncer(MushroomHandler HMushroom)
    {
        HSyncObject = HMushroom;
        StateOffset = 0x9C;
        SeparateCollisionByte = false;
        ObjectLength = 0xB0;
    }

    public override void Collect(int level)
    {
        if (Client.HGameState.IsOnMainMenuOrLoading) 
            return;
        SetMushroomState(true);
    }
    
    // SETS THE STATE OF ALL MUSHROOMS IN THE CURRENT LEVEL. SETTING FALSE WILL COMPLETELY DISABLE MUSHROOMS.
    public static void SetMushroomState(bool state)
    {
        var triggerSphereCount = Client.HSync.HTrigger.GetTriggerSphereCount();
        for (var triggerIndex = 0; triggerIndex < triggerSphereCount; triggerIndex++)
        {
            // FIND TRIGGERS WHERE ENTRY AND EXIT EVENTS TARGET THE SAME OBJECT
            var triggerEntryAddr = Client.HSync.HTrigger.GetTriggerEnterTargetAddress(triggerIndex);
            var triggerExitAddr = Client.HSync.HTrigger.GetTriggerExitTargetAddress(triggerIndex);
            if (triggerEntryAddr != triggerExitAddr)
                continue;
            // CHECK IF ENTRY AND EXIT EVENTS ARE TARGETING A TELEPORTER 
            ProcessHandler.TryRead(triggerEntryAddr + 4, out int indicatorAddr, false, "Find Teleporter Trigger");
            if (indicatorAddr != TyProcess.BaseAddress + 0x26D68C)
                continue;
            // ACTIVATE THE TRIGGER AND MAKE THE TELEPORTER VISIBLE
            Client.HSync.HTrigger.SetTriggerActivity(triggerIndex, true);
            var byteToWrite = state ? (byte)0x1 : (byte)0x0;
            ProcessHandler.WriteData(triggerEntryAddr + 0xC8, new byte[] { 0x0, byteToWrite });
        }
    }

    public static bool GetMushroomState()
    {
        ProcessHandler.TryRead(0x26D6FC, out int tpCount, true, "readTPCount");
        ProcessHandler.TryRead(0x26D700, out int tpAddr, true, "readTPAddr");
        var countActive = 0;
        for (var tpIndex = 0; tpIndex < tpCount; tpIndex++)
        {
            ProcessHandler.TryRead(tpAddr + 0xC8, out bool state1, false, "readTPAddr");
            ProcessHandler.TryRead(tpAddr + 0xC9, out bool state2, false, "readTPAddr");
            if (!state1 && state2)
                countActive++;
            ProcessHandler.TryRead(tpAddr + 0x34, out tpAddr, false, "nextTPAddr");
        }
        return countActive > 1;
    }
}