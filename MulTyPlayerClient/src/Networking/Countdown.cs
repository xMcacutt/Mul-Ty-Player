using MulTyPlayerClient.GUI;
using Riptide;
using System.Linq;
using System.Threading.Tasks;
using MulTyPlayerClient.Sync;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.Networking
{
    public class Countdown
    {
        public static void Start()
        {
            foreach (var entry in PlayerHandler.Players)
            {
                entry.Value.IsReady = false;
            }
            MainViewModel.Lobby.ReadyEnabled = false;
            MainViewModel.Lobby.UpdateReadyStatus();

            Task.Run(() =>
            {
                for (int i = 10; i > 0; i--)
                {
                    if (CheckAbort())
                    {
                        ChatLog.Write("Countdown aborted");
                        SFXPlayer.Instance.Stop();
                        MainViewModel.Lobby.ReadyEnabled = true;
                        return;
                    }
                    if (i == 10)
                    {
                        SFXPlayer.Instance.PlaySound(SFX.Race10);
                    }
                    ChatLog.Write(i.ToString());
                    if (i == 3)
                    {
                        SFXPlayer.Instance.PlaySound(SFX.Race321);
                        Replication.HSync = new SyncHandler();
                    }
                    Task.Delay(1000).Wait();
                }
                MainViewModel.Lobby.ReadyEnabled = true;
                ChatLog.Write("Go!");
            });
        }

        private static bool CheckAbort()
        {
            return !Replication.HGameState.CheckMainMenu() || MainViewModel.Lobby.PlayerInfoList.Any(p => p.Level != "M/L");
        }
    }
}
