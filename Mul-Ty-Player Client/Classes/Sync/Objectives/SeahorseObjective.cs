using System;
using System.Linq;

namespace MulTyPlayerClient.Objectives;

public class SeahorseObjective : Objective
{
    public SeahorseObjective(int level, string name) : base(level, name)
    {
        Count = 8;
        State = ObjectiveState.Inactive;
        ObjectPath = new[] { 0x257A1C, 0x0 };
        CheckValue = 4972;
        ObjectActiveState = 1;
        CurrentData = new byte[8];
        OldData = new byte[8];
        PlaySoundOnGet = true;
    }

    protected override void IsInactive()
    {
        ProcessHandler.TryRead(ObjectAddress + 0xA8, out int seahorseActivity, false, "CableCar : IsActive()");
        if (seahorseActivity != 3)
            return;
        State = ObjectiveState.Active;
        SendState();
    }

    protected override void IsActive()
    {
        Client.HSync.HTrigger.CheckSetTrigger(16, false);
        UpdateCount();
        for (var i = 0; i < Count; i++)
        {
            if (CurrentData[i] == 1)
                continue;
            ProcessHandler.WriteData(ObjectAddress + i * 0x19C + 0xA8, new byte[] { 3 });
            ProcessHandler.WriteData(ObjectAddress + i * 0x19C + 0xC0, new byte[] { 1 });
            ProcessHandler.TryRead(ObjectAddress + i * 0x19C + 0xB0, out int seahorseOutState, false, "CableCar : IsActive()");
            if (seahorseOutState is 2 or 3)
            {
                CurrentData[i] = 1;
                if (OldData[i] == 1)
                    continue;
                SendIndex(i);
                OldData[i] = 1;
            }
        }
        UpdateCount();
        if (CurrentData.Any(x => x != 1))
            return;
        State = ObjectiveState.ReadyForTurnIn;
        SendState();
    }

    protected override void IsReady()
    {
        Client.HSync.HTrigger.CheckSetTrigger(16, false);
        Client.HSync.HTrigger.CheckSetTrigger(17, true);
        if ((Client.HSync.SyncObjects["TE"].SaveSync as SaveTESyncer)?.GlobalSaveData[Level][4] != 1)
            return;
        State = ObjectiveState.Complete;
    }

    protected override void Activate()
    {
    }

    protected override void AllowTurnIn()
    {
        
    }

    protected override void Deactivate()
    {
        for (var i = 0; i < Count; i++)
        {
            ProcessHandler.TryRead(ObjectAddress + i * 0x19C + 0xA8, out int seahorseActivity, false,
                "Seahorse : IsReady()");
            if (seahorseActivity == 0x0)
                continue;
            ProcessHandler.WriteData(ObjectAddress + i * 0x19C + 0xA8, new byte[1]);
        }
    }

    protected override void UpdateCount()
    {
        CurrentCount = CurrentData.Count(x => x == ObjectActiveState);
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x260228 + 0x70, BitConverter.GetBytes(CurrentCount));
    }

    protected override void UpdateObjectState(int index)
    {
        ProcessHandler.TryRead(ObjectAddress + index * 0x19C + 0xA8, out int seahorseActivity, false, "CableCar : IsActive()");
        if (seahorseActivity == 0x0)
            return;
        ProcessHandler.WriteData(ObjectAddress + index * 0x19C + 0xA8, new byte[] { 0x0 });
    }

    public override void Sync(byte[] data)
    {
        for (var i = 0; i < Count; i++)
        {
            if (data[i] != 1 || CurrentData[i] == 1) continue;
            CurrentData[i] = OldData[i] = data[i];
            UpdateObjectState(i);
            UpdateCount();
        }
    }
}