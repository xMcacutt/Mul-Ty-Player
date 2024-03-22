using System;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.Classes.Utility;
using Vector = System.Numerics.Vector;

namespace MulTyPlayerClient;

public class SpectatorHandler
{
    public static Koala? SpectateeKoalaId = null;

    public static void SetSpectatee(int id)
    { 
        SpectateeKoalaId = (Koala)id;
    }

    public static void LookAtSpectatee()
    {
        var currentCameraPos = ReadCameraPosition();
        if (SpectateeKoalaId == null || !PlayerReplication.PlayerTransforms.TryGetValue((int)SpectateeKoalaId, out var playerTransform))
            return;
        var currentSpectateePos = playerTransform.Position.AsFloats();
        var currentCameraPosVector = new Vector3(currentCameraPos[0], currentCameraPos[1], currentCameraPos[2]);
        var currentSpectateePosVector = new Vector3(currentSpectateePos[0], currentSpectateePos[1], currentSpectateePos[2]);
        
        var positionVector = currentSpectateePosVector - currentCameraPosVector;
        
        var distanceXZ = Math.Sqrt(positionVector.X * positionVector.X + positionVector.Z * positionVector.Z);
        var yaw = -Math.Atan2(positionVector.X, positionVector.Z) + (float)Math.PI;
        var pitch = Math.Atan2(-positionVector.Y, distanceXZ);

        SetCameraRotation(Convert.ToSingle(pitch), Convert.ToSingle(yaw));
    }

    public static void FollowSpectatee()
    {
        if (!PlayerReplication.PlayerTransforms.TryGetValue((int)SpectateeKoalaId, out var playerTransform))
            return;
        if (playerTransform.LevelId != Client.HLevel.CurrentLevelId)
            Client.HLevel.ChangeLevel(playerTransform.LevelId);
    }

    public static void SetCameraPosition(float x, float y, float z)
    {
        var addr = (int)TyProcess.BaseAddress + 0x27EB78;
        float[] pos = { x, y, z };
        for (var i = 0; i < 3; i++)
            ProcessHandler.WriteData(addr + i * 4, BitConverter.GetBytes(pos[i]));
    }

    public static void SetCameraRotation(float p, float y)
    {
        var addr = (int)TyProcess.BaseAddress + 0x27F2B4;
        float[] rot = { p, y };
        rot[1] += (float)Math.PI / 2;
        for (var i = 0; i < 2; i++)
            ProcessHandler.WriteData(addr + i * 4, BitConverter.GetBytes(rot[i]));
    }

    public static float[] ReadCameraPosition()
    {
        var cameraPos = new float[3];
        for (
            var i = 0; i < 3; i++)
            ProcessHandler.TryRead(0x27EB78 + i * 4, out cameraPos[i], true, "ReadCameraPosition");
        return cameraPos;
    }

    public static void FindNextSpectatee()
    {
        if (!PlayerHandler.Players.Any(x => x.Koala == SpectateeKoalaId))
            SpectateeKoalaId = null;
        var currentLevelPlayers = PlayerHandler.Players.Where(
            x => x.Koala != null &&
                 PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId == Client.HLevel.CurrentLevelId).ToArray();
        if (SpectateeKoalaId is null && currentLevelPlayers.Length > 0)
        {
            SpectateeKoalaId = currentLevelPlayers[0].Koala;
            return;
        }
        
        if (PlayerHandler.Players.Count(x => x.Koala != null) == 1)
        {
            SpectateeKoalaId = null;
            return;
        }
        
        if (currentLevelPlayers.Length == 1)
        {
            var currentOtherLevelPlayers = PlayerHandler.Players.Where(
                x => x.Koala != null && 
                     PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId != Client.HLevel.CurrentLevelId).ToArray();
            SpectateeKoalaId = currentOtherLevelPlayers[0].Koala;
        }
        
        for (var playerIndex = 0; playerIndex < currentLevelPlayers.Length; playerIndex++)
        {
            if (currentLevelPlayers[playerIndex].Koala != SpectateeKoalaId)
                continue;
            playerIndex++;
            playerIndex %= currentLevelPlayers.Length;
            SpectateeKoalaId = currentLevelPlayers[playerIndex].Koala;
            return;
        }
    }
    
    public static void FindPreviousSpectatee()
    {
        if (!PlayerHandler.Players.Any(x => x.Koala == SpectateeKoalaId))
            SpectateeKoalaId = null;
        var currentLevelPlayers = PlayerHandler.Players.Where(
            x => x.Koala != null &&
                 PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId == Client.HLevel.CurrentLevelId).ToArray();
        if (SpectateeKoalaId is null && currentLevelPlayers.Length > 0)
        {
            SpectateeKoalaId = currentLevelPlayers[0].Koala;
            return;
        }
        
        if (PlayerHandler.Players.Count(x => x.Koala != null) == 1)
        {
            SpectateeKoalaId = null;
            return;
        }
        
        if (currentLevelPlayers.Length == 1)
        {
            var currentOtherLevelPlayers = PlayerHandler.Players.Where(
                x => x.Koala != null && 
                     PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId != Client.HLevel.CurrentLevelId).ToArray();
            SpectateeKoalaId = currentOtherLevelPlayers[0].Koala;
        }
        
        for (var playerIndex = 0; playerIndex < currentLevelPlayers.Length; playerIndex++)
        {
            if (currentLevelPlayers[playerIndex].Koala != SpectateeKoalaId)
                continue;
            playerIndex--;
            playerIndex %= currentLevelPlayers.Length;
            SpectateeKoalaId = currentLevelPlayers[playerIndex].Koala;
            return;
        }
    }
}