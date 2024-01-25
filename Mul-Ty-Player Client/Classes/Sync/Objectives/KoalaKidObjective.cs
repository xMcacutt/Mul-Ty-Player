using System;
using System.Linq;

namespace MulTyPlayerClient.Objectives;

public class KoalaKidObjective : Objective
{
    public KoalaKidObjective(int level, string name) : base(level, name)
    {
        Count = 8;
        State = ObjectiveState.Active;
        ObjectPath = new[] { 0x26A4B0, 0x0 };
        CheckValue = 26304;
        ObjectActiveState = 0x5;
        CurrentData = new byte[] {1, 1, 1, 1, 1, 1, 1, 1};
        OldData = new byte[] {1, 1, 1, 1, 1, 1, 1, 1};
    }

    protected override void IsInactive() { }

    protected override void IsActive()
    {
        ProcessHandler.TryRead(ObjectAddress + 0x6C, out ushort objectiveState, false, "KKO: IsActive() 1");
        if (objectiveState == 0)
            ProcessHandler.WriteData(ObjectAddress + 0x6C, BitConverter.GetBytes((ushort)1));
        //READ COUNT
        ProcessHandler.TryRead(ObjectAddress + 0x70, out CurrentCount, false, "KKO: IsActive() 1");
        if (CurrentCount > OldCount)
        {
            OldCount = CurrentCount;
            for (var i = 0; i < Count; i++)
            {
                //READ EACH KOALA STATE
                ProcessHandler.TryRead(ObjectAddress + 0x90 + (i * 2) * 0x518 + 0x98, out int result, false,
                    "KKO: IsActive() 2");
                if (result <= 2 || OldData[i] > 3)
                    continue;
                CurrentData[i] = OldData[i] = 7;
                //SEND MESSAGE TO SERVER
                SendIndex(i);
            }
        }
        if (CurrentCount != 8) return;
        State = ObjectiveState.ReadyForTurnIn;
    }

    protected override void IsReady()
    {
        var successIndex = Level == 9 ? 14 : 16;
        var startIndex = Level == 9 ? 18 : 17;
        Client.HSync.HTrigger.CheckSetTrigger(successIndex, true);
        Client.HSync.HTrigger.CheckSetTrigger(startIndex, false);
        ProcessHandler.TryRead(ObjectAddress + 0x6C, out ushort result, false, "KKO: IsActive() 1");
        if (result == 0x1)
            ProcessHandler.WriteData(ObjectAddress + 0x6C, BitConverter.GetBytes((ushort)0x0));
        ProcessHandler.WriteData(ObjectAddress + 0x70, new byte[] { 8 });
        //CHECK TE STATE FOR COMPLETION
        if (Client.HSync.SyncObjects["TE"].GlobalObjectData[Level][3] != 5)
            return;
        State = ObjectiveState.Complete;
        SendState();
    }

    protected override void Activate() { }

    protected override void AllowTurnIn() { }

    protected override void Deactivate() { }

    protected override void UpdateCount()
    {
    }

    protected override void UpdateObjectState(int index)
    {
        if (Client.HLevel.CurrentLevelId != Level)
            return;
        ProcessHandler.WriteData(ObjectAddress + 0x90 + (index * 2) * 0x518 + 0x44, new byte[] { 1 });
        ProcessHandler.WriteData(ObjectAddress + 0x90 + (index * 2) * 0x518 + 0x98, new byte[] { 0x5 });
    }

    public override void Sync(byte[] data)
    {
        for (var i = 0; i < Count; i++)
        {
            if (data[i] == 5)
                ProcessHandler.WriteData(ObjectAddress + 0x90 + (i * 2) * 0x518 + 0x44, new byte[] { 1 });
            if (data[i] == 1)
                continue;
            ProcessHandler.WriteData(ObjectAddress + 0x90 + (i * 2) * 0x518 + 0x98, new byte[] { data[i] });
        }
        OldCount = CurrentCount = data.Count(x => x == ObjectActiveState);
        UpdateCount();
    }
}