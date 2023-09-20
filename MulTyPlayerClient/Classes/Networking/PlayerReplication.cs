using Iced.Intel;
using MulTyPlayerClient.Classes.GamePlay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MulTyPlayerClient.Classes.Networking
{
    using KoalaID = System.Int32;
    internal static class PlayerReplication
    {
        public readonly static KoalaInterpolationMode InterpolationMode = KoalaInterpolationMode.Interpolate;

        const int RENDER_CALLS_PER_CLIENT_TICK = 8;
        const int KRENDER_SLEEP_TIME = (int)((float)Client.MS_PER_TICK / RENDER_CALLS_PER_CLIENT_TICK) - 1;
        static Dictionary<KoalaID, Transform> playerTransforms;
        static Dictionary<KoalaID, TransformSnapshots> receivedSnapshotData;

        static CancellationTokenSource renderTokenSource;

        static PlayerReplication()
        {
            receivedSnapshotData = new();
            playerTransforms = new();
        }

        public static Task RenderKoalas(int maxTimeMilliseconds)
        {
            if (InterpolationMode == KoalaInterpolationMode.None)
            {
                RenderTick();
                Thread.Sleep(Client.MS_PER_TICK);
                return Task.CompletedTask;
            }
            renderTokenSource = new CancellationTokenSource();
            var renderToken = renderTokenSource.Token;
            Task render = Task.Run(() =>
            {
                //Debug.WriteLine("RENDERING!");
                for (int i = 0; i < RENDER_CALLS_PER_CLIENT_TICK; i++)
                {
                    RenderTick();
                    Thread.Sleep(KRENDER_SLEEP_TIME);
                }
            }, renderToken);
            Task renderAsync = render.WaitAsync(renderToken);
            renderTokenSource.CancelAfter(maxTimeMilliseconds);
            return renderAsync;
        }

        public static void CancelRender()
        {
            renderTokenSource?.Cancel();
            renderTokenSource?.Dispose();
        }

        public static void RenderTick()
        {
            UpdateTransforms();
            WritePlayersTransformData();            
        }

        #region AddRemovePlayers
        public static void AddPlayer(KoalaID koalaID)
        {
            receivedSnapshotData.Remove(koalaID);
            receivedSnapshotData.Add(koalaID, new());
            playerTransforms.Remove(koalaID);
            playerTransforms.Add(koalaID, new());
        }

        public static void RemovePlayer(KoalaID koalaID)
        {
            receivedSnapshotData.Remove(koalaID);
            playerTransforms.Remove(koalaID);
        }

        public static void ClearPlayers()
        {
            receivedSnapshotData.Clear();
            playerTransforms.Clear();
        }
        #endregion

        static void UpdateTransforms()
        {
            foreach (KoalaID koalaID in receivedSnapshotData.Keys)
            {
                var snapshots = receivedSnapshotData[koalaID];
                playerTransforms[koalaID].Position = Interpolation.LerpPosition(snapshots, InterpolationMode);
                playerTransforms[koalaID].Rotation = snapshots.New.Transform.Rotation;
            }
        }
        
        static void WritePlayersTransformData()
        {
            foreach (KoalaID koalaID in receivedSnapshotData.Keys)
            {
                KoalaHandler.KoalaTransformAddresses ktp = KoalaHandler.TransformAddresses[koalaID];
                Transform transform = playerTransforms[koalaID];

                ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(transform.Position.X));
                ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(transform.Position.Y));
                ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(transform.Position.Z));
                ProcessHandler.WriteData(ktp.Pitch, BitConverter.GetBytes(transform.Rotation.Pitch));
                ProcessHandler.WriteData(ktp.Yaw, BitConverter.GetBytes(transform.Rotation.Yaw));
                ProcessHandler.WriteData(ktp.Roll, BitConverter.GetBytes(transform.Rotation.Roll));
            }
        }

        internal static void UpdatePlayerSnapshotData(KoalaID koalaID, float[] transform)
        {
            receivedSnapshotData[koalaID].Update(new(transform));
        }
    }
}
