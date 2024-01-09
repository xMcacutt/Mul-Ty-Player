using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

internal class PortalHandler : SyncObjectHandler
{
    public static int[] LivePortalOrder = { 7, 5, 4, 13, 10, 23, 20, 19, 9, 21, 22, 12, 8, 6, 14, 15 };
    public static int[] FlakyPortals = { 4, 7, 15, 19, 21, 22, 23 };
    public Dictionary<int, byte> OldPortalsActive;
    public Dictionary<int, byte> PortalsActive;

    public PortalHandler()
    {
        Name = "Portal";
        WriteState = 1;
        CheckState = 3;
        CounterByteLength = 1;
        PreviousObserverState = 0;
        ObserverState = 0;
        LiveSync = new LivePortalSyncer(this);
        SetSyncClasses(LiveSync, SaveSync);
        PortalsActive = new Dictionary<int, byte>();
        OldPortalsActive = new Dictionary<int, byte>();
        foreach (var level in FlakyPortals)
        {
            PortalsActive.Add(level, 0);
            OldPortalsActive.Add(level, 0);
        }
    }

    public override void HandleClientUpdate(int null1, int null2, int level)
    {
        PortalsActive[level] = 1;
        if (Client.HLevel.CurrentLevelId != 0) return;
        LiveSync.Collect(level);
    }

    public override void Sync(int null1, byte[] active, byte[] null2)
    {
        for (var i = 0; i < 7; i++)
            if (active[i] == 1)
                HandleClientUpdate(null1, null1, FlakyPortals[i]);
    }

    public override int ReadObserver(int address, int size)
    {
        var portals = new int[7];
        //THERE ARE 7 PORTALS WHICH ARE HIDDEN BY DEFAULT IN RAINBOW CLIFFS
        for (var i = 0; i < 7; i++)
        {
            //CHECKS IF THE PORTAL SHOULD BE ACTIVE BECAUSE THE LEVEL HAS BEEN ENTERED ALREADY
            ProcessHandler.TryRead(SyncHandler.SaveDataBaseAddress + 0x70 * FlakyPortals[i],
                out byte portalSaveDataState, false, "PortalHandler::ReadObserver() 1");
            if (portalSaveDataState > 0 || PortalsActive[FlakyPortals[i]] > 0)
            {
                PortalsActive[FlakyPortals[i]] = 1;
                portals[i] = 1;
            }

            //CHECKS IF THE PORTAL SHOULD BE ACTIVE BECAUSE IT IS LOADED IN RAINBOW CLIFFS
            if (Client.HLevel.CurrentLevelId == 0)
            {
                //ORDER OF PORTALS IS IRRITATING SO I JUST MADE A LIST OF THEM
                var orderedIndex = Array.IndexOf(LivePortalOrder, FlakyPortals[i]);
                ProcessHandler.TryRead(address + LiveSync.ObjectLength * orderedIndex, out byte portalLiveDataState,
                    false, "PortalHandler::ReadObserver() 2");
                if (portalLiveDataState == 2)
                {
                    PortalsActive[FlakyPortals[i]] = 1;
                    portals[i] = 1;
                }
            }
        }

        return portals.Count(i => i == 1);
    }

    public override void CheckObserverChanged()
    {
        ObserverState = ReadObserver(LiveObjectAddress + LiveSync.StateOffset, CounterByteLength);
        if (PreviousObserverState == ObserverState || ObserverState == 0) return;

        PreviousObserverState = ObserverState;

        foreach (var i in FlakyPortals)
            if (OldPortalsActive[i] == 0 && PortalsActive[i] == 1)
                Client.HSync.SendDataToServer(i, i, i, Name);
        foreach (var i in OldPortalsActive.Keys) OldPortalsActive[i] = PortalsActive[i];
    }

    public override void SetMemAddrs()
    {
        LiveObjectAddress = PointerCalculations.GetPointerAddress(0x267408, new[] { 0x0 });
        ProcessHandler.CheckAddress(LiveObjectAddress, (ushort)(17321904 & 0xFFFF), "Portal base address check");
    }
}