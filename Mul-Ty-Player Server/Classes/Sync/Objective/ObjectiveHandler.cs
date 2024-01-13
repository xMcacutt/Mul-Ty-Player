using System.Collections.Generic;
using MulTyPlayer;
using MulTyPlayerClient.Objectives;
using MulTyPlayerServer.Classes;
using Riptide;

namespace MulTyPlayerServer.Sync.Objective;

public class ObjectiveHandler
{
    private static Dictionary<string, Objective> Objectives;
    
    public ObjectiveHandler()
    {
        Objectives = new Dictionary<string, Objective>
        {
            { "Burner", new Objective("Burner", 8, 1, 2) },
            { "SnowKoalaKid", new Objective("SnowKoalaKid", 8, 1, 2) },
            { "StumpKoalaKid", new Objective("StumpKoalaKid", 8, 1, 2) }

        };
    }

    [MessageHandler((ushort)MessageID.ReqObjectiveSync)]
    private static void HandleSyncRequest(ushort fromClientId, Message message)
    {
        foreach (var objective in Objectives)
        {
            var sync = Message.Create(MessageSendMode.Reliable, MessageID.ReqObjectiveSync);
            sync.AddString(objective.Key);
            sync.AddBytes(objective.Value.ObjectStates);
            sync.AddByte((byte)objective.Value.State);
            Server._Server.Send(sync, fromClientId);
        }
    }

    [MessageHandler((ushort)MessageID.ObjectiveObjectActivated)]
    private static void HandleObjectActivated(ushort fromClientId, Message message)
    {
        var type = message.GetString();
        var index = message.GetInt();
        Objectives[type].SetObjectActivated(index, fromClientId);
    }
    
    [MessageHandler((ushort)MessageID.ObjectiveStateChanged)]
    private static void HandleObjectiveStateChanged(ushort fromClientId, Message message)
    {
        var type = message.GetString();
        var state = message.GetByte();
        Objectives[type].SetObjectiveState((ObjectiveState)state, fromClientId);
    }
}