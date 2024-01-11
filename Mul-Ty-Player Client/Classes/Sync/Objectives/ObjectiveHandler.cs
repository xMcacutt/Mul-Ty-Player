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
    public static BurnerObjective HBurner;
    public Dictionary<string, Objective> Objectives;

    public ObjectiveHandler()
    {
        Objectives = new Dictionary<string, Objective>
        {
            { "Burner", HBurner = new BurnerObjective() }
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
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqObjectiveSync);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.ReqObjectiveSync)]
    private static void HandleSyncReqResponse(Message message)
    {
        if (Client.Relaunching) return;
        var type = message.GetString();
        Client.HObjective.Objectives[type].CurrentData = message.GetBytes();
        Client.HObjective.Objectives[type].State = (ObjectiveState)message.GetByte();
    }
    
    public void RunChecks()
    {
        foreach (var objective in Objectives.Values.Where(objective => objective.Level == Client.HLevel.CurrentLevelId))
            objective.RunCheck();
    }
}