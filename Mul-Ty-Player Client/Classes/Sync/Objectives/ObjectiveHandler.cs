using System;
using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.LevelLock;
using Riptide;

namespace MulTyPlayerClient.Objectives;

public class ObjectiveHandler
{
    public Dictionary<string, Objective> Objectives;

    public ObjectiveHandler()
    {
        Objectives = new Dictionary<string, Objective>
        {
            { "Burner", new BurnerObjective(8, "Burner") },
            { "SnowKoalaKid", new KoalaKidObjective(9, "SnowKoalaKid") },
            { "StumpKoalaKid", new KoalaKidObjective(13, "StumpKoalaKid") },
            { "Chest", new ChestObjective(14, "Chest")}
        };
    }

    public void SetMemAddrs()
    {
        foreach (var objective in Objectives.Values)
            if (Client.HLevel.CurrentLevelId == objective.Level)
                objective.SetMemoryAddress();
    }

    public void SendIndexToServer(int index, string type)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ObjectiveObjectActivated);
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
        //Console.WriteLine($"Activated {type} number {index}");
        Client.HObjective.Objectives[type].SetObjectActive(index);
    }

    [MessageHandler((ushort)MessageID.ObjectiveStateChanged)]
    public static void HandleStateChanged(Message message)
    {
        var type = message.GetString();
        var state = (ObjectiveState)message.GetByte();
        //Console.WriteLine($"{type} state changed to {Enum.GetName(typeof(ObjectiveState), state)}");
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
        foreach (var objective in Objectives.Values.Where(objective => objective.Level == Client.HLevel.CurrentLevelId))
            objective.RunCheck();
    }
}