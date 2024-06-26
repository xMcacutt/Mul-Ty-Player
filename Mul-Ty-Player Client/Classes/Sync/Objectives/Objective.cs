﻿using System;
using System.Linq;
using Riptide;

namespace MulTyPlayerClient.Objectives;

public class ObjectiveStateChangedEventArgs : EventArgs
{
    public ObjectiveState OldState { get; }
    public ObjectiveState NewState { get; }

    public ObjectiveStateChangedEventArgs(ObjectiveState oldState, ObjectiveState newState)
    {
        OldState = oldState;
        NewState = newState;
    }
} 

public abstract class Objective
{
    public string Name;
    public int Count;
    public int Level;
    public int CurrentCount;
    public int OldCount;
    public int[] ObjectPath;
    public int ObjectAddress;
    public byte[] CurrentData;
    public byte[] OldData;
    public byte ObjectActiveState;
    public ushort CheckValue;
    public bool PlaySoundOnGet;

    private ObjectiveState state;
    public ObjectiveState State
    {
        get => state;
        set
        {
            if (state == value) return;
            var oldState = state;
            state = value;
            OnStateChanged?.Invoke(this, 
                new ObjectiveStateChangedEventArgs(oldState, 
                     state));
        }
    }
    
    public event EventHandler<ObjectiveStateChangedEventArgs> OnStateChanged;

    protected Objective(int level, string name)
    {
        Name = name;
        Level = level;
        OnStateChanged += RunAction;
    }
    
    public void RunCheck()
    {
        if (!MemoryAddressValid())
        {
            SetMemoryAddress();
            return;
        }
        switch (State)
        {
            case ObjectiveState.Inactive:
                IsInactive();
                break;
            case ObjectiveState.Active:
                IsActive();
                break;
            case ObjectiveState.ReadyForTurnIn:
                IsReady();
                break;
            case ObjectiveState.Complete:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RunAction(object sender, ObjectiveStateChangedEventArgs e)
    {
        if (Client.HLevel.CurrentLevelId != Level)
            return;
        switch (e.NewState)
        {
            case ObjectiveState.Active:
                Activate();
                break;
            case ObjectiveState.ReadyForTurnIn:
                AllowTurnIn();
                break;
            case ObjectiveState.Complete:
                Deactivate();
                break;
            case ObjectiveState.Inactive:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool MemoryAddressValid()
    {
        ProcessHandler.TryRead(ObjectAddress, 
            out ushort result, 
            false, 
            "MemoryAddressValid()");
        return result == CheckValue;
    }

    public virtual void SetMemoryAddress()
    {
        ObjectAddress = PointerCalculations.GetPointerAddress(ObjectPath[0], ObjectPath.Skip(1).ToArray());
    }

    protected abstract void IsInactive();

    protected abstract void IsActive();

    protected abstract void IsReady();

    protected abstract void Activate();

    protected abstract void AllowTurnIn();

    protected abstract void Deactivate();

    protected abstract void UpdateCount();

    protected abstract void UpdateObjectState(int index);

    public abstract void Sync(byte[] data);
    
    public void SetObjectActive(int index)
    {
        if (CurrentData[index] == ObjectActiveState)
            return;
        OldData[index] = CurrentData[index] = ObjectActiveState;
        OldCount = CurrentCount = CurrentData.Count(x => x == ObjectActiveState);
        if (Client.HLevel.CurrentLevelId != Level)
            return;
        if (PlaySoundOnGet)
        {
            SFXPlayer.StopAll();
            SFXPlayer.PlaySound(SFX.Objective);
        }
        UpdateObjectState(index);
        UpdateCount();
    }

    public void SetState(ObjectiveState newState)
    {
        if (State != newState)
            State = newState;
    }

    public void SendIndex(int index)
    {
        Client.HObjective.SendIndexToServer(index, Name);
    }

    public void SendState()
    {
        Client.HObjective.SendObjectiveStateToServer(State, Name);
    }
}
