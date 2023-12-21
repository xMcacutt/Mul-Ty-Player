﻿namespace MulTyPlayerClient;

internal class LiveCogSyncer : LiveDataSyncer
{
    public LiveCogSyncer(CogHandler hCog)
    {
        HSyncObject = hCog;
        StateOffset = 0xC4;
        SeparateCollisionByte = false;
        ObjectLength = 0x144;
    }
}