﻿using System;

namespace MulTyPlayerClient;

internal class LivePortalSyncer : LiveDataSyncer
{
    public LivePortalSyncer(PortalHandler HPortal)
    {
        HSyncObject = HPortal;
        StateOffset = 0x9C;
        SeparateCollisionByte = false;
        ObjectLength = 0xB0;
    }

    public override void Collect(int level)
    {
        if (Client.HGameState.IsOnMainMenuOrLoading) return;
        var portalIndex = Array.IndexOf(PortalHandler.LivePortalOrder, level);
        ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + ObjectLength * portalIndex,
            new[] { HSyncObject.WriteState }, "Making portal visible");
    }
}