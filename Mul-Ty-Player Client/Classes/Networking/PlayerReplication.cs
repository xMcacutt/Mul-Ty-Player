using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayerClient.Classes.GamePlay;

namespace MulTyPlayerClient.Classes.Networking;

using KoalaID = Int32;

internal static class PlayerReplication
{
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
        foreach (var koalaID in receivedSnapshotData.Keys)
            if (!Client.HGameState.IsAtMainMenuOrLoading() && PlayerTransforms[koalaID].LevelID == Client.HLevel.CurrentLevelId)
            {
                var t = UpdateTransform(koalaID);
                WriteTransformData(koalaID, t);
            }
    }

    private static Transform UpdateTransform(KoalaID koalaID)
    {
        var snapshots = receivedSnapshotData[koalaID];
        PlayerTransforms[koalaID].Position = Interpolation.LerpPosition(snapshots, InterpolationMode);
        PlayerTransforms[koalaID].Rotation = snapshots.New.Transform.Rotation;
        PlayerTransforms[koalaID].LevelID = snapshots.New.Transform.LevelID;
        return PlayerTransforms[koalaID];
    }

    private static void WriteTransformData(KoalaID koalaID, Transform transform)
    {
        if (transform.LevelID != Client.HLevel.CurrentLevelId)
            return;

        var ktp = KoalaHandler.TransformAddresses[koalaID];

        ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(transform.Position.X));
        ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(transform.Position.Y));
        ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(transform.Position.Z));
        ProcessHandler.WriteData(ktp.Pitch, BitConverter.GetBytes(transform.Rotation.Pitch));
        ProcessHandler.WriteData(ktp.Yaw, BitConverter.GetBytes(transform.Rotation.Yaw));
        ProcessHandler.WriteData(ktp.Roll, BitConverter.GetBytes(transform.Rotation.Roll));
    }

    internal static void UpdatePlayerSnapshotData(KoalaID koalaID, float[] transform, int levelID)
    {
        receivedSnapshotData[koalaID].Update(new TransformSnapshot(transform, levelID));
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