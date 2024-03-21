using System;
using System.Numerics;
using System.Windows.Documents;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.Classes.Utility;

namespace MulTyPlayerClient;

public class SpectatorHandler
{
    public static Koala SpectateeKoalaId = 0;
    public static bool LookingAtSpectatee = true;

    public static void SetSpectatee(int id)
    { 
        SpectateeKoalaId = (Koala)id;
    }

    public static void LookAtSpectatee()
    {
        var currentCameraPos = ReadCameraPosition();
        if (!PlayerReplication.PlayerTransforms.TryGetValue((int)SpectateeKoalaId, out var playerTransform))
            return;
        var currentSpectateePos = playerTransform.Position.AsFloats();
        var cameraPos = Array.ConvertAll(currentCameraPos, x => (double)x);
        var spectateePos = Array.ConvertAll(currentSpectateePos, x => (double)x);
        var pitch = Convert.ToSingle(VectorMath.CalculatePitch(cameraPos, spectateePos));
        var yaw = Convert.ToSingle(VectorMath.CalculateYaw(cameraPos, spectateePos));
        SetCameraRotation(pitch, yaw);
    }

    public static void FollowSpectatee()
    {
        
    }

    public static void SetCameraPosition(float x, float y, float z)
    {
        var addr = (int)TyProcess.BaseAddress + 0x27EB78;
        float[] pos = { x, y, z };
        for (var i = 0; i < 3; i++)
            ProcessHandler.WriteData(addr + i, BitConverter.GetBytes(pos[i]));
    }

    public static void SetCameraRotation(float p, float y)
    {
        var addr = (int)TyProcess.BaseAddress + 0x27F2B4;
        float[] rot = { p, y };
        for (var i = 0; i < 2; i++)
            ProcessHandler.WriteData(addr + i, BitConverter.GetBytes(rot[i]));
    }

    public static float[] ReadCameraPosition()
    {
        var cameraPos = new float[3];
        for (var i = 0; i < 3; i++)
            ProcessHandler.TryRead(0x27F2B4, out cameraPos[i], true, "ReadCameraPosition");
        return cameraPos;
        
        
    }
}