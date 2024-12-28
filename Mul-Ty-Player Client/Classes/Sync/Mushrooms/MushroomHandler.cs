using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient;

internal class MushroomHandler : SyncObjectHandler
{
    public Dictionary<int, bool> MushroomsActive;

    public MushroomHandler()
    {
        Name = "Mushroom";
        WriteState = 1;
        CheckState = 1;
        CounterByteLength = 1;
        PreviousObserverState = 0;
        ObserverState = 0;
        LiveSync = new LiveMushroomSyncer(this);
        SetSyncClasses(LiveSync, SaveSync);
        MushroomsActive = new Dictionary<int, bool>();
        foreach (var level in Levels.MainStages)
        {
            MushroomsActive.Add(level.Id, false);
        }
    }
    
    public static void CheckMushrooms()
    {
        if (SyncHandler.HMushroom.MushroomsActive.TryGetValue(Client.HLevel.CurrentLevelId, out var state) && state)
            LiveMushroomSyncer.SetMushroomState(true);
    }

    public override void HandleClientUpdate(int null1, int null2, int level)
    {
        MushroomsActive[level] = true;
        if (Client.HLevel.CurrentLevelId != level) 
            return;
        LiveSync.Collect(level);
    }

    public override void Sync(int null1, byte[] null2, byte[] active)
    {
        foreach (var level in from level in MushroomsActive.Keys 
                 let index = Array.IndexOf(Levels.MainStages, Levels.GetLevelData(level)) 
                 where active[index] == 1 select level)
            HandleClientUpdate(null1, null1, level);
    }

    public override void CheckObserverChanged()
    {
        var state = LiveMushroomSyncer.GetMushroomState();
        if (!state || MushroomsActive[Client.HLevel.CurrentLevelId])
            return; 
        MushroomsActive[Client.HLevel.CurrentLevelId] = true;
        Client.HSync.SendDataToServer(0, 0, Client.HLevel.CurrentLevelId, Name);
    }
}