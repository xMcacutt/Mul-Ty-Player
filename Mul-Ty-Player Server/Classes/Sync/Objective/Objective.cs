using System.Linq;
using MulTyPlayer;
using MulTyPlayerClient.Objectives;
using Riptide;

namespace MulTyPlayerServer.Sync.Objective;

public class Objective
{
    public string Name;
    public byte[] ObjectStates;
    public ObjectiveState State;
    public int Count;
    public byte InitialObjectState;
    public byte ActivatedObjectState;
    
    public Objective(string name, int count, byte initialObjectState, byte activatedObjectState)
    {
        Name = name;
        Count = count;
        State = ObjectiveState.Inactive;
        InitialObjectState = initialObjectState;
        ActivatedObjectState = activatedObjectState;
        ObjectStates = Enumerable.Repeat(InitialObjectState, Count).ToArray();
    }

    public void SetObjectActivated(int index, ushort from)
    {
        ObjectStates[index] = ActivatedObjectState;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ObjectiveObjectActivated);
        message.AddString(Name);
        message.AddInt(index);
        Server._Server.SendToAll(message, from);
    }
    
    public void SetObjectiveState(ObjectiveState state, ushort from)
    {
        State = state;
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ObjectiveObjectActivated);
        message.AddString(Name);
        message.AddInt((byte)State);
        Server._Server.SendToAll(message, from);
    }
}