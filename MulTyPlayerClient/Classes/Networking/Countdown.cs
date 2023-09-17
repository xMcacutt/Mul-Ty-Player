using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes
{
    public class Countdown
    {
        public static event Action OnCountdownBegan;
        public static event Action OnCountdownAborted;
        public static event Action OnCountdownFinished;

        public static bool InProgress { get { return inProgress; } }
        private static bool inProgress = false;

        private static CancellationTokenSource abortTokenSource = new();

        [MessageHandler((ushort)MessageID.Countdown)]
        public static async void StartCountdown(Message message)
        {
            abortTokenSource = new CancellationTokenSource();
            CancellationToken abortToken = abortTokenSource.Token;

            foreach(var entry in PlayerHandler.Players)
            {
                entry.Value.IsReady = false;
            }
            ModelController.Lobby.IsReady = false;
            ModelController.Lobby.UpdateReadyStatus();
            ModelController.Lobby.OnLogout += Abort;

            var countdown = Task.Run(() =>
            {
                abortToken.ThrowIfCancellationRequested();
                OnCountdownBegan?.Invoke();
                SFXPlayer.StopAll();
                SFXPlayer.PlaySound(SFX.Race10);
                for (int i = 10; i > 0; i--)
                {
                    if (CheckAnyPlayerOnMainMenu())
                    {
                        Abort();
                    }

                    if (abortToken.IsCancellationRequested)
                    {                        
                        abortToken.ThrowIfCancellationRequested();
                    }
                    
                    Logger.Instance.Write(i.ToString());
                    if (i == 3)
                    {
                        SFXPlayer.StopAll();
                        SFXPlayer.PlaySound(SFX.Race321);
                        Client.HSync = new SyncHandler();
                    }
                    Task.Delay(1000).Wait();
                }                
                Logger.Instance.Write("Go!");
            }, abortToken);

            try
            {
                await countdown;
                OnCountdownFinished?.Invoke();
            }
            catch (OperationCanceledException cancel)
            {
                Logger.Instance.Write("Countdown aborted.");
            }
            finally
            {
                abortTokenSource.Dispose();
                ModelController.Lobby.IsReady = true;
                ModelController.Lobby.OnLogout -= Abort;
                inProgress = false;
            }
        }

        private static void Abort()
        {
            SFXPlayer.StopSound(SFX.Race10);
            SFXPlayer.StopSound(SFX.Race321);
            abortTokenSource.Cancel();
            OnCountdownAborted?.Invoke();
        }

        private static bool CheckAnyPlayerOnMainMenu()
        {
            return !Client.HGameState.IsAtMainMenu() || ModelController.Lobby.PlayerInfoList.Any(p => p.Level != "M/L");
        }
    }
}
