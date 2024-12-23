﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using MulTyPlayerClient.Classes.GamePlay;

namespace MulTyPlayerClient.Classes.Networking;

using KoalaID = Int32;

internal static class PlayerReplication
{
    private static readonly float[] _defaultKoalaPosX =
    {
        250, 0, 0, 0, -2989, -8940, -13646, -572, -3242, -518, -14213, 0, -4246, -5499, -1615, 90, 0, -166, -2989, -192,
        -8845, -82, -82, 10
    };
    private static readonly float[] _defaultKoalaPosY =
    {
        1700, 0, 0, 0, -500, -2153, -338, -1600, -1309, -4827, 4000, 0, -773, -2708, -1488, -789, 0, -100, -500, -3000, 1000, -1524,
        -1524, -200
    };
    private static readonly float[] _defaultKoalaPosZ =
    {
        6400, 0, 0, 0, 8238, 7162, 22715, -59, 6197, 212, 16627, 0, 1343, -6951, 811, 93, 0, -7041, 8238, 3264, 17487, 449,
        449, -250
    };
    
    private const int RENDER_CALLS_PER_CLIENT_TICK = 8;
    private const int KRENDER_SLEEP_TIME = (int)((float)Client.MS_PER_TICK / RENDER_CALLS_PER_CLIENT_TICK) - 1;
    public static KoalaInterpolationMode InterpolationMode = (KoalaInterpolationMode)Enum.Parse(typeof(KoalaInterpolationMode), SettingsHandler.ClientSettings.InterpolationMode);
    public static readonly Dictionary<KoalaID, Transform> PlayerTransforms;
    private static readonly Dictionary<KoalaID, TransformSnapshots> receivedSnapshotData;

    private static CancellationTokenSource renderTokenSource;

    static PlayerReplication()
    {
        receivedSnapshotData = new Dictionary<int, TransformSnapshots>();
        PlayerTransforms = new Dictionary<int, Transform>();
    }

    public static void ClearSnapshotData()
    {
        receivedSnapshotData.Clear();
    }

    public static void RenderKoalas(int maxTimeMilliseconds)
    {
        if (InterpolationMode == KoalaInterpolationMode.None)
        {
            RenderTick();
            Thread.Sleep(Client.MS_PER_TICK);
            return;
        }

        renderTokenSource = new CancellationTokenSource();
        var renderToken = renderTokenSource.Token;
        renderTokenSource.CancelAfter(maxTimeMilliseconds);

        var renderTimer = new Timer(RenderTimerCallback, renderToken, 0, KRENDER_SLEEP_TIME);

        try
        {
            renderToken.WaitHandle.WaitOne(); // Wait until canceled
        }
        finally
        {
            renderTimer.Dispose();
        }
    }

    private static void RenderTimerCallback(object state)
    {
        var cancellationToken = (CancellationToken)state;

        for (var i = 0; i < RENDER_CALLS_PER_CLIENT_TICK; i++)
        {
            RenderTick();
        }
    }

    public static void CancelRender()
    {
        renderTokenSource?.Cancel();
        renderTokenSource?.Dispose();
    }

    public static void RenderTick()
    {
        foreach (var koalaId in receivedSnapshotData.Keys)
            if (!Client.HGameState.IsOnMainMenuOrLoading && !Client.Relaunching)
            {
                var t = UpdateTransform(koalaId);
                WriteTransformData(koalaId, t);
            }
    }

    private static Transform UpdateTransform(KoalaID koalaId)
    {
        var snapshots = receivedSnapshotData[koalaId];
        PlayerTransforms[koalaId].Position = Interpolation.LerpPosition(snapshots, InterpolationMode);
        PlayerTransforms[koalaId].Rotation = snapshots.New.Transform.Rotation;
        PlayerTransforms[koalaId].OnMenu = snapshots.New.Transform.OnMenu;
        PlayerTransforms[koalaId].LevelId = snapshots.New.Transform.LevelId;
        return PlayerTransforms[koalaId];
    }

    public static void ReturnKoala(KoalaID koalaId)
    {
        var level = PlayerTransforms[koalaId].LevelId;
        if (Client.HLevel.CurrentLevelId != level)
            return;
        var ktp = KoalaHandler.TransformAddresses[koalaId];
        PlayerTransforms[koalaId].Position = new Position(_defaultKoalaPosX[level], _defaultKoalaPosY[level], _defaultKoalaPosZ[level]);
        ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(_defaultKoalaPosX[level]));
        ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(_defaultKoalaPosY[level]));
        ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(_defaultKoalaPosZ[level]));
        Client.HGlow.ReturnGlow(koalaId);
    }

    private static void WriteTransformData(KoalaID koalaId, Transform transform)
    {
        var ktp = KoalaHandler.TransformAddresses[koalaId];
        var gtp = GlowHandler.TransformAddresses[koalaId];

        var selfLevel = Client.HLevel.CurrentLevelId;
        if (transform.LevelId != selfLevel)
        {
            PlayerTransforms[koalaId].Position = new Position(_defaultKoalaPosX[selfLevel], _defaultKoalaPosY[selfLevel], _defaultKoalaPosZ[selfLevel]);
            ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(_defaultKoalaPosX[selfLevel]));
            ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(_defaultKoalaPosY[selfLevel]));
            ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(_defaultKoalaPosZ[selfLevel]));
            Client.HGlow.ReturnGlow(koalaId);
            return;
        }
        if (transform.OnMenu)
        {
            ReturnKoala(koalaId);
            return;
        }
        ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(transform.Position.X));
        ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(transform.Position.Y));
        ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(transform.Position.Z));
        ProcessHandler.WriteData(ktp.Pitch, BitConverter.GetBytes(transform.Rotation.Pitch));
        ProcessHandler.WriteData(ktp.Yaw, BitConverter.GetBytes(transform.Rotation.Yaw));
        ProcessHandler.WriteData(ktp.Roll, BitConverter.GetBytes(transform.Rotation.Roll));

        if ((SettingsHandler.GameMode == GameMode.HideSeek || !SettingsHandler.ClientSettings.ShowKoalaBeacons) && !Client.HHideSeek.LinesVisible)
            return;
        
        ProcessHandler.WriteData(gtp.X, BitConverter.GetBytes(transform.Position.X));
        ProcessHandler.WriteData(gtp.Y, BitConverter.GetBytes(transform.Position.Y + 200));
        ProcessHandler.WriteData(gtp.Z, BitConverter.GetBytes(transform.Position.Z));
        ProcessHandler.WriteData(gtp.sX, BitConverter.GetBytes(0.3f), "Scaling glows");
        ProcessHandler.WriteData(gtp.sY, BitConverter.GetBytes(40f), "Scaling glows");
        ProcessHandler.WriteData(gtp.sZ, BitConverter.GetBytes(0.3f), "Scaling glows");
    }

    internal static void UpdatePlayerSnapshotData(KoalaID koalaId, float[] transform, bool onMenu, int levelId)
    {
        receivedSnapshotData[koalaId].Update(new TransformSnapshot(transform, onMenu, levelId));
    }

    #region AddRemovePlayers

    public static void AddPlayer(KoalaID koalaID)
    {
        receivedSnapshotData.Remove(koalaID);
        receivedSnapshotData.Add(koalaID, new TransformSnapshots());
        PlayerTransforms.Remove(koalaID);
        PlayerTransforms.Add(koalaID, new Transform());
    }

    public static void RemovePlayer(KoalaID koalaID)
    {
        ReturnKoala(koalaID);
        receivedSnapshotData.Remove(koalaID);
        PlayerTransforms.Remove(koalaID);
    }

    public static void ClearPlayers()
    {
        receivedSnapshotData.Clear();
        PlayerTransforms.Clear();
    }

    #endregion
}