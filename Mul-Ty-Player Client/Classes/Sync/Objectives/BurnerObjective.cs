using System;
using System.Linq;
using System.Management;
using Riptide;

namespace MulTyPlayerClient.Objectives;

public class BurnerObjective : Objective
{
    public BurnerObjective(int level, string name) : base(level, name)
    {
        Count = 8;
        State = ObjectiveState.Inactive;
        ObjectPath = new[] { 0x2681E8, 0x0 };
        CheckValue = 21704;
        ObjectActiveState = 0x2;
        CurrentData = new byte[] {1, 1, 1, 1, 1, 1, 1, 1};
        OldData = new byte[] {1, 1, 1, 1, 1, 1, 1, 1};
        PlaySoundOnGet = true;
    }

    protected override void IsInactive()
    {
        //MONITOR OBJ STATE
        ProcessHandler.TryRead(ObjectAddress + 0x6C, out ushort objectiveState, false, "Burner: IsInactive()");
        if (objectiveState == 0)
            return;
        //WHEN ACTIVATED SET STATE TO ACTIVE
        State = ObjectiveState.Active;
        SendState();
    }

    protected override void IsActive()
    {
        //WHILE ACTIVE CHECK OBJ STATE
        ProcessHandler.TryRead(ObjectAddress + 0x6C, out ushort objectiveState, false, "Burner: IsActive() 1");
        if (objectiveState == 0)
            Activate();
        Client.HSync.HTrigger.CheckSetTrigger(15, false);
        //READ COUNT
        ProcessHandler.TryRead(ObjectAddress + 0x70, out CurrentCount, false, "Burner: IsActive() 2");
        if (CurrentCount > OldCount)
        {
            OldCount = CurrentCount;
            for (var i = 0; i < Count; i++)
            {
                //READ EACH BURNER STATE
                ProcessHandler.TryRead(ObjectAddress + 0x90 + i * 0x70 + 0x6C, out CurrentData[i], false, "Burner: IsActive() 3");
                if (CurrentData[i] != ObjectActiveState || OldData[i] == ObjectActiveState) continue;
                OldData[i] = CurrentData[i];
                //SEND MESSAGE TO SERVER
                SendIndex(i);
            }
        }
        if (CurrentCount != 8) return;
        State = ObjectiveState.ReadyForTurnIn;
        SendState();
    }

    protected override void IsReady()
    {
        Client.HSync.HTrigger.CheckSetTrigger(15, false);
        Client.HSync.HTrigger.CheckSetTrigger(19, true);
        ProcessHandler.TryRead(ObjectAddress + 0x6C, out ushort objectiveState, false, "Burner: IsReady() 1");
        if (objectiveState == 1)
            AllowTurnIn();
        //CHECK TE STATE FOR COMPLETION
        if (Client.HSync.SyncObjects["TE"].GlobalObjectData[Level][4] != 5)
            return;
        State = ObjectiveState.Complete;
        SendState();
    }

    protected override void Activate()
    {
        //ACTIVATE STATE
        ProcessHandler.WriteData(ObjectAddress + 0x6C, new byte[] { 0x1 });
        ProcessHandler.WriteData(ObjectAddress + 0x6E, new byte[] { 0x8 });
        for (var i = 0; i < Count; i++)
        {
            ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * i) + 0x4C, new byte[] { 0x1 });
            ProcessHandler.TryRead(ObjectAddress + 0x90 + (0x70 * i) + 0x6C, out ushort objectState, false,
                "Burner: Activate() 1");
            if (objectState == 0)
                ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * i) + 0x6C, new byte[] { 0x1 });
        }
    }

    protected override void AllowTurnIn()
    {
        ProcessHandler.WriteData(ObjectAddress + 0x6C, new byte[] { 0x0 });
        for (var i = 0; i < 8; i++)
        {
            ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * i) + 0x4C, new byte[] { 0x1 });
            ProcessHandler.TryRead(ObjectAddress + 0x90 + (0x70 * i) + 0x6C, out ushort objectState, false,
                "Burner: AllowTurnIn() 1");
            if (objectState == 0)
                ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * i) + 0x6C, new byte[] { 0x2 });
        }
    }

    protected override void Deactivate()
    {
        Client.HSync.HTrigger.CheckSetTrigger(19, false);
    }

    protected override void UpdateCount()
    {
        ProcessHandler.WriteData(ObjectAddress + 0x70, BitConverter.GetBytes(CurrentCount));
    }

    protected override void UpdateObjectState(int index)
    {
        ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * index) + 0x6C, new byte[] { 0x2 });
    }

    public override void Sync(byte[] data)
    {
        for (var i = 0; i < Count; i++)
            ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * i) + 0x6C, new byte[] { data[i] });
        OldCount = CurrentCount = data.Count(x => x == ObjectActiveState);
        UpdateCount();
    }
}