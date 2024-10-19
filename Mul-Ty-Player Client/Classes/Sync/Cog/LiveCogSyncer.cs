using System;

namespace MulTyPlayerClient;

internal class LiveCogSyncer : LiveDataSyncer
{
    public LiveCogSyncer(CogHandler hCog)
    {
        HSyncObject = hCog;
        StateOffset = 0xC4;
        SeparateCollisionByte = false;
        ObjectLength = 0x144;
    }

    public override void Collect(int index)
    {
        if (Client.HGameState.IsOnMainMenuOrLoading) 
            return;
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + ObjectLength * index,
            new[] { HSyncObject.WriteState }, "Setting collectible to collected");
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x265270, BitConverter.GetBytes(0xA0));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x26526C, BitConverter.GetBytes(0x1));
        if (!SeparateCollisionByte) 
            return;
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + CollisionOffset + ObjectLength * index,
            BitConverter.GetBytes(0), "Setting collision of collectible to off");
    }
}