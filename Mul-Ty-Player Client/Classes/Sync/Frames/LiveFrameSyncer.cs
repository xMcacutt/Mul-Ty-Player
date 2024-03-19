using System;

namespace MulTyPlayerClient;

internal class LiveFrameSyncer : LiveDataSyncer
{
    public LiveFrameSyncer(FrameHandler HFrame)
    {
        HSyncObject = HFrame;
    }

    private FrameHandler HFrame => HSyncObject as FrameHandler;
    private static LevelHandler HLevel => Client.HLevel;

    public (int, byte)[] ReadData()
    {
        var framesInLevel = Levels.GetLevelData(HLevel.CurrentLevelId).FrameCount;
        var currentFrames = new (int, byte)[framesInLevel];

        var address = HFrame.NonCrateFrameAddress;
        ProcessHandler.TryRead(0x26BEAC, out int nonCrateFrameCount, true, "Non Crate Frames Count");

        for (var i = 0; i < nonCrateFrameCount; i++)
        {
            currentFrames[i] = ReadFrame(address);
            address += 0xCC;
        }
        
        address = HFrame.CrateFrameAddress;
        for (var i = nonCrateFrameCount; i < framesInLevel; i++)
        {
            currentFrames[i] = ReadFrame(address);
            ProcessHandler.TryRead(address + 0x30, out address, false, "Next Frame");
        }
        
        return currentFrames;
    }

    private (int, byte) ReadFrame(int address)
    {
        (int, byte) frameData;
        ProcessHandler.TryRead(address + 0x84, out frameData.Item1, false,
            "Non Crate Frame Index");
        ProcessHandler.TryRead(address + 0x8A, out frameData.Item2, false,
            "Non Crate Frame State");
        return frameData;
    }

    public override void Sync(byte[] bytes, int amount, int checkState)
    {
        for (var i = 0; i < amount; i++)
        {
            if (bytes[i] == checkState)
            {
                Collect(i);
            }
        }
    }

    public override void Collect(int index)
    {
        // IF INDEX IS LESS THAN NON CRATE FRAME COUNT, THEN TREAT AS NON CRATE FRAME NBO MANIPULATION
        Console.WriteLine(index);
        if (Client.HGameState.IsOnMainMenuOrLoading) return;
        var framesInLevel = Levels.GetLevelData(HLevel.CurrentLevelId).FrameCount;
        var address = HFrame.CrateFrameAddress;
        ProcessHandler.TryRead(0x26BEAC, out int nonCrateFrameCount, true, "Non Crate Frames Count");
        if (index < nonCrateFrameCount)
        {
            address = HFrame.NonCrateFrameAddress + 0xCC * index;
        }
        else
        {
            index -= framesInLevel - nonCrateFrameCount;

            for (var i = 0; i < index; i++)
                ProcessHandler.TryRead(address + 0x30, out address, false, "LiveFrameSyncer::Collect {0}");
        }
        ProcessHandler.WriteData(address + 0x89, new byte[] { 0x1 }, "LiveFrameSyncer::Collect {1}");
        ProcessHandler.WriteData(address + 0x8B, new byte[] { 0x1 }, "LiveFrameSyncer::Collect {1}");
    }
}