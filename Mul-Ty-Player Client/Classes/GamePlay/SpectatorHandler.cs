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
    private static Koala? _spectateeKoalaId;
    public static Koala? SpectateeKoalaId {
        get => _spectateeKoalaId;
        set
        {
            _spectateeKoalaId = value;
            ChangedLevel = false;
        }
    }

    private static Vector3 _currentSpectateeVector;
    private static Vector3 _currentCameraVector;
    private static Vector3 _currentDisplacementVector;
    private static float _cameraDistance;
    private static int _spectateeLevel;

    public static void UpdateCamera()
    {
        if (SpectateeKoalaId != null)
        {
            if (!PlayerHandler.Players.Any(x => x.Koala == SpectateeKoalaId))
                SpectateeKoalaId = null;
            if (SpectateeKoalaId == null || 
                !PlayerReplication.PlayerTransforms.TryGetValue((int)SpectateeKoalaId, out var spectateeInfo) ||
                spectateeInfo.OnMenu)
                return;
            _spectateeLevel = spectateeInfo.LevelId;
            var currentSpectateePos = spectateeInfo.Position;
            _currentSpectateeVector = new Vector3(currentSpectateePos.X, currentSpectateePos.Y, currentSpectateePos.Z);
            _currentCameraVector = ReadCameraPosition();
            _currentDisplacementVector = _currentSpectateeVector - _currentCameraVector;
            _cameraDistance = Vector3.Distance(_currentSpectateeVector, _currentCameraVector);
            FollowSpectatee();
            LookAtSpectatee();
        }
    }
    
    public static void LookAtSpectatee()
    {
        var flatDistance = Math.Sqrt(_currentDisplacementVector.X * _currentDisplacementVector.X + _currentDisplacementVector.Z * _currentDisplacementVector.Z);
        var yaw = -Math.Atan2(_currentDisplacementVector.X, _currentDisplacementVector.Z) + Math.PI;
        var pitch = Math.Atan2(-_currentDisplacementVector.Y, flatDistance);
        SetCameraRotation(Convert.ToSingle(pitch), Convert.ToSingle(yaw));
    }

    public static bool ChangedLevel = false;
    public static void FollowSpectatee()
    {
        if (_cameraDistance > 5000)
        {
            var newCameraVector = _currentSpectateeVector - _currentDisplacementVector * 0.1f;
            SetCameraPosition(newCameraVector.X, newCameraVector.Y, newCameraVector.Z);
        }
        if (_spectateeLevel != Client.HLevel.CurrentLevelId && !ChangedLevel)
        {
            Client.HLevel.ChangeLevel(_spectateeLevel);
            ChangedLevel = true;
        }
    }

    public static void SetCameraPosition(float x, float y, float z)
    {
        var addr = (int)TyProcess.BaseAddress + 0x27EB78;
        float[] pos = { x, y, z };
        for (var i = 0; i < 3; i++)
            ProcessHandler.WriteData(addr + i * 4, BitConverter.GetBytes(pos[i]));
    }

    public static void SetCameraRotation(double p, double y)
    {
        var addr = (int)TyProcess.BaseAddress + 0x27F2B4;
        double[] rot = { p, y };
        rot[1] += Math.PI / 2;
        for (var i = 0; i < 2; i++)
            ProcessHandler.WriteData(addr + i * 4, BitConverter.GetBytes(Convert.ToSingle(rot[i])));
    }

    public static float[] ReadCameraRotation()
    {
        var cameraRot = new float[2];
        for (
            var i = 0; i < 2; i++)
            ProcessHandler.TryRead(0x27F2B4 + i * 4, out cameraRot[i], true, "ReadCameraPosition");
        return cameraRot;
    }
    
    public static Vector3 ReadCameraPosition()
    {
        Vector3 cameraPos;
        ProcessHandler.TryRead(0x27EB78 + 0, out cameraPos.X, true, "ReadCameraPosition");
        ProcessHandler.TryRead(0x27EB78 + 4, out cameraPos.Y, true, "ReadCameraPosition");
        ProcessHandler.TryRead(0x27EB78 + 8, out cameraPos.Z, true, "ReadCameraPosition");
        return cameraPos;
    }

    public static void FindNextSpectatee()
    {
        var currentLevelPlayers = PlayerHandler.Players.Where(
            x => x.Koala != null &&
                       PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId == Client.HLevel.CurrentLevelId).ToArray();
        if (SpectateeKoalaId is null && currentLevelPlayers.Length > 0)
        {
            SpectateeKoalaId = currentLevelPlayers[0].Koala;
            return;
        }
        
        if (currentLevelPlayers.Length <= 1)
        {
            var currentOtherLevelPlayers = PlayerHandler.Players.Where(
                x => x.Koala != null && 
                           PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId != Client.HLevel.CurrentLevelId).ToArray();
            SpectateeKoalaId = currentOtherLevelPlayers.Length == 0 ? null : currentOtherLevelPlayers[0].Koala;
            return;
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

        if (PlayerHandler.Players.Count(x => x.Koala != null) != 1) 
            return;
        SpectateeKoalaId = null;
    }
    
    public static void FindPreviousSpectatee()
    {
        var currentLevelPlayers = PlayerHandler.Players.Where(
            x => x.Koala != null &&
                 PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId == Client.HLevel.CurrentLevelId).ToArray();
        if (SpectateeKoalaId is null && currentLevelPlayers.Length > 0)
        {
            SpectateeKoalaId = currentLevelPlayers[currentLevelPlayers.Length - 1].Koala;
            return;
        }
        
        if (currentLevelPlayers.Length <= 1)
        {
            var currentOtherLevelPlayers = PlayerHandler.Players.Where(
                x => x.Koala != null && 
                     PlayerReplication.PlayerTransforms[(int)x.Koala].LevelId != Client.HLevel.CurrentLevelId).ToArray();
            SpectateeKoalaId = currentOtherLevelPlayers.Length == 0 ? null : currentOtherLevelPlayers[currentOtherLevelPlayers.Length - 1].Koala;
            return;
        }
        
        for (var playerIndex = 0; playerIndex < currentLevelPlayers.Length; playerIndex++)
        {
            if (currentLevelPlayers[playerIndex].Koala != SpectateeKoalaId)
                continue;
            playerIndex--;
            playerIndex = (playerIndex + currentLevelPlayers.Length) % currentLevelPlayers.Length;
            SpectateeKoalaId = currentLevelPlayers[playerIndex].Koala;
            return;
        }

        if (PlayerHandler.Players.Count(x => x.Koala != null) != 1) 
            return;
        SpectateeKoalaId = null;
    }


    public static bool _inFreeCam = false;
    public static void ToggleFreeCam(bool state = false, bool forceState = false)
    {
        ProcessHandler.TryRead(0x27EBD0, out int currentCameraState, true, "ReadCamState");
        _inFreeCam = currentCameraState == 28;
        if (state == _inFreeCam && forceState)
            return;
        if (forceState)
            _inFreeCam = !state;
        var newCamState = _inFreeCam ? 5 : 28;
        var newHeroState = 5;
        if (Client.HLevel.CurrentLevelData.Id == Levels.OutbackSafari.Id)
        {
            newHeroState = _inFreeCam ? 0 :  5;
        }
        else
        {
            newHeroState = _inFreeCam ? 35 :  50;
        }
        if (_inFreeCam)
        {
            var camPos = SpectatorHandler.ReadCameraPosition();
            Client.HHero.WritePosition(camPos.X, camPos.Y, camPos.Z);
        }
        Client.HGameState.SetCameraState(newCamState);
        Client.HHero.SetHeroState(newHeroState);
        if (!_inFreeCam)
            SpectatorHandler.SetCameraRotation(0, Client.HHero.GetCurrentPosRot()[4]);
        _inFreeCam = !_inFreeCam;
    }
}