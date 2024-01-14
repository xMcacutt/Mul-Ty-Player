using System;
using System.Linq;

namespace MulTyPlayerClient.Objectives;

public class ChestObjective : Objective
{
    private static TriggerHandler HTrigger => Client.HSync.HTrigger;
    private int CurrentChest;
    private static int[] ChestTriggerIndices = new [] { 2, 3, 5, 6, 4, 7 };
    
    public ChestObjective(int level, string name) : base(level, name)
    {
        Count = 6;
        State = ObjectiveState.Inactive;
        ObjectPath = new[] { 0x26DAA0, 0x0 };
        CheckValue = 31492;
        CurrentData = new byte[6];
        CurrentChest = (int)Chest.CrabIsland;
        OldData = new byte[6];
    }

    protected override void IsInactive()
    {
        if (HTrigger.GetTriggerActivity(ChestTriggerIndices[(int)Chest.CrabIsland]) == 0)
            return;
        State = ObjectiveState.Active;
        SendState();
    }

    protected override void IsActive()
    {
        //SET ACTIVATION TRIGGER TO OFF
        HTrigger.CheckSetTrigger(0, false);
        //GET CURRENT CHEST FROM DATA COUNT
        CurrentChest = CurrentData.Count(x => x == 1) - 1;
        //IF ALL CHESTS ARE ACTIVE SET TURN IN READY
        if (CurrentChest == 5)
        {
            State = ObjectiveState.ReadyForTurnIn;
            return;
        }
        //SET TRIGGER AND CHEST VISIBILITY FOR ALL PREVIOUS CHESTS
        for (var i = 0; i < CurrentChest - 1; i++)
        {
            HTrigger.CheckSetTrigger(ChestTriggerIndices[i], false);
            ChestHandler.CheckSetVisibility(ChestHandler.ChestObjectIndices[i], true);
        }
        //SET CURRENT CHEST TRIGGER AND CHEST VISIBILITY
        HTrigger.CheckSetTrigger(ChestTriggerIndices[CurrentChest], true);
        ChestHandler.CheckSetVisibility(ChestHandler.ChestObjectIndices[CurrentChest], true);
        //CHECK NEXT CHEST TRIGGER FOR ACTIVITY
        if (HTrigger.GetTriggerActivity(ChestTriggerIndices[CurrentChest + 1]) == 0)
            return;
        //IF ACTIVE SEND INDEX AND UPDATE DATA
        CurrentData[CurrentChest + 1] = 1;
        SendIndex(CurrentChest + 1);
    }

    protected override void IsReady()
    {
        //TURN MAIN TRIGGER OFF
        HTrigger.CheckSetTrigger(0, false);
        //TURN ALL TRIGGERS OFF BUT CHESTS ON
        for (var i = 0; i < 5; i++)
        {
            HTrigger.CheckSetTrigger(ChestTriggerIndices[i], false);
            ChestHandler.CheckSetVisibility(ChestHandler.ChestObjectIndices[i], true);
        }
        //TURN LAST CHEST AND TRIGGER ON
        HTrigger.CheckSetTrigger(ChestTriggerIndices[5], true);
        ChestHandler.CheckSetVisibility(ChestHandler.ChestObjectIndices[5], true);
        if (Client.HSync.SyncObjects["TE"].GlobalObjectData[Level][3] != 5)
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
    }

    protected override void UpdateCount()
    {
    }

    protected override void UpdateObjectState(int index)
    {
        CurrentData[index] = 1;
    }

    public override void Sync(byte[] data)
    {
        Array.Copy(data, CurrentData, data.Length);
    }
}

public enum Chest : int
{
    CrabIsland,
    AnchorRock,
    BaldIsland,
    CoconutShores,
    JaneysGrave,
    PirateCove
}

public class ChestHandler
{
    public static int[] ChestObjectIndices = new [] { 4, 0, 1, 2, 3, 5 };

    public static void CheckSetVisibility(int index, bool value)
    {
        var valueByte = value ? (byte)7 : (byte)3;
        var result = GetChestVisibility(index);
        if (result == valueByte)
            return;
        SetChestVisibility(index, value);
    }
    
    public static byte GetChestVisibility(int index)
    {
        ProcessHandler.TryRead(Client.HObjective.Objectives["Chest"].ObjectAddress + 0xB8 * index + 0x94,
            out byte result, false, "ChestHandler: GetChestVisibility()");
        return result;
    }

    public static void SetChestVisibility(int index, bool value)
    {
        var visibilityByte = value ? (byte)0x7 : (byte)0x3;
        var activeByte = value ? (byte)0x1 : (byte)0x0;
        var baseAddr = Client.HObjective.Objectives["Chest"].ObjectAddress + 0xB8 * index;
        ProcessHandler.WriteData(baseAddr + 0x94, new byte[] { visibilityByte });
        ProcessHandler.WriteData(baseAddr + 0x48, new byte[] { activeByte });
        ProcessHandler.WriteData(baseAddr + 0xAC, new byte[] { activeByte });
        ProcessHandler.WriteData(baseAddr + 0xAD, new byte[] { activeByte });
        
    }
}