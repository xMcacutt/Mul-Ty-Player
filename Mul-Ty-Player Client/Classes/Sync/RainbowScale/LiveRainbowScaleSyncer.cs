using System;

namespace MulTyPlayerClient;

internal class LiveRSSyncer : LiveDataSyncer
{
    public LiveRSSyncer(RSHandler HRainbowScale)
    {
        HSyncObject = HRainbowScale;
        StateOffset = 0x78;
        SeparateCollisionByte = false;
        ObjectLength = 0x114;
    }

    private RSHandler HRainbowScale => HSyncObject as RSHandler;
    private static LevelHandler HLevel => Client.HLevel;

    public override void Collect(int index)
    {
        //If gem has already been collected, return
        if (HRainbowScale.CurrentObjectData[index] >= 3)
            return;

        //If in menu or loading, return
        if (Client.HGameState.IsAtMainMenuOrLoading())
            return;

        if (HLevel.CurrentLevelId != Levels.RainbowCliffs.Id)
            return;

        var address = HRainbowScale.LiveObjectAddress + StateOffset + ObjectLength * index;
        ProcessHandler.TryRead(address, out HRainbowScale.CurrentObjectData[index], false,
            "LiveRainbowScaleSyncer::Collect()");

        if (HRainbowScale.CurrentObjectData[index] >= 3)
            return;

        ProcessHandler.WriteData(address, BitConverter.GetBytes(3), $"Collecting opal {index}");
    }
}