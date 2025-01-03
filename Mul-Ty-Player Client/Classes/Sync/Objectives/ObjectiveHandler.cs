﻿using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient.Objectives;

public class ObjectiveHandler
{
    public Dictionary<string, Objective> Objectives = new()
    {
        { "Seahorse", new SeahorseObjective(6, "Seahorse")},
        { "Burner", new BurnerObjective(8, "Burner") },
        { "SnowKoalaKid", new KoalaKidObjective(9, "SnowKoalaKid") },
        { "StumpKoalaKid", new KoalaKidObjective(13, "StumpKoalaKid") },
        { "CableCar", new CableCarObjective(13, "CableCar")},
        { "Chest", new ChestObjective(14, "Chest")}
    };

    public void SetMemAddrs()
    {
        foreach (var objective in Objectives.Values.Where(x => Client.HLevel.CurrentLevelId == x.Level))
            objective.SetMemoryAddress();
    }

    public void SendIndexToServer(int index, string type)
    {
        var message = Message.Create(MessageSendMode.Reliable, 
            MessageID.ObjectiveObjectActivated);
        message.AddString(type);
        message.AddInt(index);
        Client._client.Send(message);
    }
    
    public void SendObjectiveStateToServer(ObjectiveState state, string type)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ObjectiveStateChanged);
        message.AddString(type);
        message.AddByte((byte)state);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.ObjectiveObjectActivated)]
    public static void HandleObjectActivated(Message message)
    {
        var type = message.GetString();
        var index = message.GetInt();
        Client.HObjective.Objectives[type].SetObjectActive(index);
    }

    [MessageHandler((ushort)MessageID.ObjectiveStateChanged)]
    public static void HandleStateChanged(Message message)
    {
        var type = message.GetString();
        var state = (ObjectiveState)message.GetByte();
        Client.HObjective.Objectives[type].SetState(state);
    }
    
    public void RequestSync()
    {
        if (!SettingsHandler.DoTESyncing) return;
        SetMemAddrs();
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqObjectiveSync);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.ReqObjectiveSync)]
    private static void HandleSyncReqResponse(Message message)
    {
        if (Client.Relaunching) return;
        var type = message.GetString();
        var objects = message.GetBytes();
        var state = message.GetByte();
        
        Array.Copy(objects, Client.HObjective.Objectives[type].OldData, objects.Length);
        Array.Copy(objects, Client.HObjective.Objectives[type].CurrentData, objects.Length);
        Client.HObjective.Objectives[type].State = (ObjectiveState)state;
        if (Client.HLevel.CurrentLevelId == Client.HObjective.Objectives[type].Level)
            Client.HObjective.Objectives[type].Sync(objects);
    }
    
    public void RunChecks()
    {
        foreach (var objective in Objectives.Values.Where(objective => 
                     objective.Level == Client.HLevel.CurrentLevelId))
            objective.RunCheck();
    }

    public void SetPerimeterCheckHealth(int health)
    {
        if (health > 6)
            return;
        var count = 6 - health;
        var addr = PointerCalculations.GetPointerAddress(0x265608, new[] { 0x390 });
        ProcessHandler.WriteData(addr, BitConverter.GetBytes(count));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x265074, BitConverter.GetBytes(health));
    }
    
    public int GetPerimeterCheckHealth()
    {
        var addr = PointerCalculations.GetPointerAddress(0x265608, new[] { 0x390 });
        ProcessHandler.TryRead(addr, out int count, false, "getPerimCheckHealth");
        ProcessHandler.TryRead(0x265074, out int health2, true, "getPerimCheckHealth");
        var health1 = 6 - count;
        return int.Max(health1, health2);
    }
}