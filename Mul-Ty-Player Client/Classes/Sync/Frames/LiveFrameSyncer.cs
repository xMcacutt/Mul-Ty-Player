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

        var address = HFrame.FrameAddress;
        for (var i = 0; i < framesInLevel; i++)
        {
            ProcessHandler.TryRead(address + 0x84, out currentFrames[i].Item1, false,
                "LiveFrameSyncer::ReadData() {0}");
            ProcessHandler.TryRead(address + 0x8A, out currentFrames[i].Item2, false,
                "LiveFrameSyncer::ReadData() {1}");
            ProcessHandler.TryRead(address + 0x30, out address, false, "LiveFrameSyncer::ReadData() {2}");
        }

        return currentFrames;
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
        if (Client.HGameState.IsAtMainMenuOrLoading()) return;
        var address = HFrame.FrameAddress;
        for (var i = 0; i < index; i++)
            ProcessHandler.TryRead(address + 0x30, out address, false, "LiveFrameSyncer::Collect {0}");
        ProcessHandler.WriteData(address + 0x89, new byte[] { 0x1 }, "LiveFrameSyncer::Collect {1}");
        ProcessHandler.WriteData(address + 0x8B, new byte[] { 0x1 }, "LiveFrameSyncer::Collect {1}");
    }
}