using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayerClient.Classes.GamePlay;

namespace MulTyPlayerClient.Classes.Networking;

using KoalaID = Int32;

internal static class PlayerReplication
{
    private static readonly float[] _defaultKoalaPosX =
    {
        250, 0, 0, 0, -2989, -8940, -13646, -572, -3242, -518, -14213, 0, -4246, -5499, -1615, 90, 0, -166, 0, -192,
        -8845, -82, -82, 10
    };
    private static readonly float[] _defaultKoalaPosY =
    {
        1700, 0, 0, 0, -500, -2153, -338, -1600, -1309, -4827, 4000, 0, -773, -2708, -1488, -789, 0, -100, 0, -3000, 1000, -1524,
        -1524, -200
    };
    private static readonly float[] _defaultKoalaPosZ =
    {
        6400, 0, 0, 0, 8238, 7162, 22715, -59, 6197, 212, 16627, 0, 1343, -6951, 811, 93, 0, -7041, 0, 3264, 17487, 449,
        449, -250
    };
    
    private const int RENDER_CALLS_PER_CLIENT_TICK = 8;
    private const int KRENDER_SLEEP_TIME = (int)((float)Client.MS_PER_TICK / RENDER_CALLS_PER_CLIENT_TICK) - 1;
    public static KoalaInterpolationMode InterpolationMode = (KoalaInterpolationMode)Enum.Parse(typeof(KoalaInterpolationMode), SettingsHandler.Settings.InterpolationMode);
    public static readonly Dictionary<KoalaID, Transform> PlayerTransforms;
    private static readonly Dictionary<KoalaID, TransformSnapshots> receivedSnapshotData;

    private static CancellationTokenSource renderTokenSource;

    static PlayerReplication()
    {
        receivedSnapshotData = new Dictionary<int, TransformSnapshots>();
        PlayerTransforms = new Dictionary<int, Transform>();
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
            if (!Client.HGameState.IsOnMainMenuOrLoading)
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
        PlayerTransforms[koalaId].LevelID = snapshots.New.Transform.LevelID;
        return PlayerTransforms[koalaId];
    }

    public static void ReturnKoala(KoalaID koalaId)
    {
        var level = PlayerTransforms[koalaId].LevelID;
        if (Client.HLevel.CurrentLevelId != level)
            return;
        var ktp = KoalaHandler.TransformAddresses[koalaId];
        PlayerTransforms[koalaId].Position = new Position(_defaultKoalaPosX[level], _defaultKoalaPosY[level], _defaultKoalaPosZ[level]);

        ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(_defaultKoalaPosX[level]));
        ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(_defaultKoalaPosY[level]));
        ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(_defaultKoalaPosZ[level]));
    }

    private static void WriteTransformData(KoalaID koalaId, Transform transform)
    {
        var ktp = KoalaHandler.TransformAddresses[koalaId];

        var selfLevel = Client.HLevel.CurrentLevelId;
        Console.WriteLine(transform.Position.X);
        if (transform.LevelID != selfLevel)
        {
            PlayerTransforms[koalaId].Position = new Position(_defaultKoalaPosX[selfLevel], _defaultKoalaPosY[selfLevel], _defaultKoalaPosZ[selfLevel]);
            ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(_defaultKoalaPosX[selfLevel]));
            ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(_defaultKoalaPosY[selfLevel]));
            ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(_defaultKoalaPosZ[selfLevel]));
            return;
        }

        ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(transform.Position.X));
        ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(transform.Position.Y));
        ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(transform.Position.Z));
        ProcessHandler.WriteData(ktp.Pitch, BitConverter.GetBytes(transform.Rotation.Pitch));
        ProcessHandler.WriteData(ktp.Yaw, BitConverter.GetBytes(transform.Rotation.Yaw));
        ProcessHandler.WriteData(ktp.Roll, BitConverter.GetBytes(transform.Rotation.Roll));
    }

    internal static void UpdatePlayerSnapshotData(KoalaID koalaId, float[] transform, int levelId)
    {
        receivedSnapshotData[koalaId].Update(new TransformSnapshot(transform, levelId));
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