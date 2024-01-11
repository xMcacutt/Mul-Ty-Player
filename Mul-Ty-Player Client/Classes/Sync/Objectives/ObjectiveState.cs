namespace MulTyPlayerClient.Objectives;

public enum ObjectiveState : byte
{
    Inactive = 0,
    Active = 1,
    ReadyForTurnIn = 2,
    Complete = 3
}