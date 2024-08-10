namespace MulTyPlayerClient.Classes.GamePlay
{
    public enum CameraState
    {
        Default = 5,
        Scope = 8,
        CameraOverride = 12,
        FluffyRotationLock = 13,
        Locked = 17,
        Cutscene = 18, // DONT DO IT
        RexDiving = 20, // CAN BE USED TO UN-SOFTLOCK WHEN SWAPPED!!!
        FreeCam = 28,
    }
}
