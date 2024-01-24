using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace MulTyPlayerClient.Objectives;

public class CableCarObjective : Objective
{
    public int[] ObjectivePath = new[] { 0x258968, 0x0 };
    public int[] RockFrillIndices = new[] { 8, 9, 10, 49, 58, 59 }; 
    
    public CableCarObjective(int level, string name) : base(level, name)
    {
        Count = 6;
        State = ObjectiveState.Active;
        ObjectPath = new[] { 0x25AAD0, 0x0 };
        CheckValue = 56264;
        ObjectActiveState = 0x8;
        CurrentData = new byte[] {0, 0, 0, 0, 0, 0};
        OldData = new byte[] {0, 0, 0, 0, 0, 0};
    }

    protected override void IsInactive()
    {
        var address = PointerCalculations.GetPointerAddress(ObjectivePath[0], new[] { 0x6C });
        ProcessHandler.TryRead(address, out int objectiveActivity, false, "CableCar : IsInactive()");
        if (objectiveActivity != 0x1)
            return;
        State = ObjectiveState.Active;
        SendState();
    }

    protected override void IsActive()
    {
        Client.HSync.HTrigger.CheckSetTrigger(1, false);
        for (var i = 0; i < Count; i++)
        {
            if (CurrentData[i] == 1)
                continue;
            var frillIndex = RockFrillIndices[i];
            ProcessHandler.TryRead(ObjectAddress + frillIndex * 0x438 + 0xA0, out int frillOutState, false, "CableCar : IsActive()");
            if (frillOutState == 8)
            {
                CurrentData[i] = 1;
                if (OldData[i] == 1)
                    continue;
                SendIndex(i);
                OldData[i] = 1;
            }
            else
            {
                ProcessHandler.TryRead(ObjectAddress + frillIndex * 0x438 + 0x48, out int frillActivity, false, "CableCar : IsActive()");
                if (frillActivity == 0x3)
                    continue;
                ProcessHandler.WriteData(ObjectAddress + frillIndex * 0x438 + 0x44,
                    new byte[] { 0x3, 0x2, 0x0, 0x0, 0x3, 0x0, 0x0, 0x0 });
            }
        }
        if (CurrentData.All(x => x == 0x1))
            State = ObjectiveState.ReadyForTurnIn;
    }

    protected override void IsReady()
    {
        Client.HSync.HTrigger.CheckSetTrigger(1, false);
        Client.HSync.HTrigger.CheckSetTrigger(2, true);
        if (Client.HSync.SyncObjects["TE"].GlobalObjectData[Level][4] != 5)
            return;
        State = ObjectiveState.Complete;
    }

    protected override void Activate()
    {
    }

    protected override void AllowTurnIn()
    {
        for (var i = 0; i < Count; i++)
        {
            var frillIndex = RockFrillIndices[i];
            ProcessHandler.TryRead(ObjectAddress + frillIndex * 0x438 + 0x48, out int frillActivity, false,
                "CableCar : IsReady()");
            if (frillActivity == 0x3)
                continue;
            ProcessHandler.WriteData(ObjectAddress + frillIndex * 0x438 + 0x44,
                new byte[] { 0x0, 0x2, 0x0, 0x0, 0x3, 0x0, 0x0, 0x0 });
        }
    }

    protected override void Deactivate()
    {
        Client.HSync.HTrigger.CheckSetTrigger(1, false);
        Client.HSync.HTrigger.CheckSetTrigger(2, false);
    }

    protected override void UpdateCount()
    {
    }

    protected override void UpdateObjectState(int index)
    {
    }

    public override void Sync(byte[] data)
    {
    }
}