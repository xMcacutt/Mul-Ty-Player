namespace MulTyPlayerClient.Objectives;

public class SeahorseObjective : Objective
{
    public SeahorseObjective(int level, string name) : base(level, name)
    {
        Count = 8;
        State = ObjectiveState.Inactive;
        ObjectPath = new[] { 0x257A1C, 0x0 };
        CheckValue = 4972;
        CurrentData = new byte[8];
        OldData = new byte[8];
    }

    protected override void IsInactive()
    {
        ProcessHandler.TryRead(0x260228 + 0x6C, out ushort objectiveActivity, true, "Seahorse: IsInactive()");
        if (objectiveActivity == 0)
            return;
        State = ObjectiveState.Active;
        SendState();
    }

    protected override void IsActive()
    {
        for (var i = 0; i < Count; i++)
        {
            if (CurrentData[i] == 1)
                continue;
            ProcessHandler.TryRead(ObjectAddress + i * 0x19C + 01, out int seahorseOutState, false, "CableCar : IsActive()");
            if (seahorseOutState == 8)
            {
                CurrentData[i] = 1;
                if (OldData[i] == 1)
                    continue;
                SendIndex(i);
                OldData[i] = 1;
            }
        }
    }

    protected override void IsReady()
    {
        throw new System.NotImplementedException();
    }

    protected override void Activate()
    {
        throw new System.NotImplementedException();
    }

    protected override void AllowTurnIn()
    {
        throw new System.NotImplementedException();
    }

    protected override void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateCount()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateObjectState(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Sync(byte[] data)
    {
        throw new System.NotImplementedException();
    }
}