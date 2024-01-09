using System;
using System.Windows.Markup;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

internal class LiveTESyncer : LiveDataSyncer
{
    public LiveTESyncer(TEHandler hThEg)
    {
        HSyncObject = hThEg;
        StateOffset = 0xC4;
        SeparateCollisionByte = false;
        ObjectLength = 0x144;
    }
}