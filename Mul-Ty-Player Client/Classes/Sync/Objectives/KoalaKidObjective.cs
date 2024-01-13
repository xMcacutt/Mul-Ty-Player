using System;
using System.Linq;

namespace MulTyPlayerClient.Objectives;

public class KoalaKidObjective : Objective
{
    public KoalaKidObjective(int level) : base(level)
    {
        Name = "SnowKoalaKid";
        Count = 8;
        State = ObjectiveState.Active;
        ObjectPath = new[] { 0x26A4B0, 0x0 };
        ObjectActiveState = 0x5;
        CurrentData = new byte[] {1, 1, 1, 1, 1, 1, 1, 1};
        OldData = new byte[] {1, 1, 1, 1, 1, 1, 1, 1};
    }
    
    protected override void IsInactive() { }

    protected override void IsActive()
    {
        //READ COUNT
        ProcessHandler.TryRead(ObjectAddress + 0x70, out CurrentCount, false, "KKO: IsActive() 1");
        if (CurrentCount > OldCount)
        {
            OldCount = CurrentCount;
            for (var i = 0; i < Count; i++)
            {
                if (CurrentData[i] == 7) continue;
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
        SendState();
    }

    protected override void IsReady()
    {
        ProcessHandler.TryRead(ObjectAddress + 0x6C, out ushort result, false, "KKO: IsActive() 1");
        if (result == 0x1)
            ProcessHandler.WriteData(ObjectAddress + 0x6C, BitConverter.GetBytes((ushort)0x0));
        //CHECK TE STATE FOR COMPLETION
        if (Client.HSync.SyncObjects["TE"].GlobalObjectData[Level][4] != 5)
            return;
        State = ObjectiveState.Complete;
        SendState();
    }

    protected override void Activate() { }

    protected override void AllowTurnIn() { }

    protected override void Deactivate() { }

    protected override void UpdateCount()
    {
        ProcessHandler.WriteData(ObjectAddress + 0x70, BitConverter.GetBytes(CurrentCount));
    }

    protected override void UpdateObjectState(int index)
    {
        ProcessHandler.WriteData(ObjectAddress + 0x90 + (index * 2) * 0x518 + 0x98, new byte[] { 0x5 });
    }

    public override void Sync(byte[] data)
    {
        for (var i = 0; i < Count; i++)
            ProcessHandler.WriteData(ObjectAddress + 0x90 + (0x70 * i) + 0x6C, new byte[] { data[i] });
        OldCount = CurrentCount = data.Count(x => x == ObjectActiveState);
        UpdateCount();
    }
}