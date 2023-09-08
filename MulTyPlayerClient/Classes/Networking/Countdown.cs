using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MulTyPlayerClient.Classes
{
    public class Countdown
    {
        
        [MessageHandler((ushort)MessageID.Countdown)]
        public static void StartCountdown(Message message)
        {
            foreach(var entry in PlayerHandler.Players)
            {
                entry.Value.IsReady = false;
            }
            ModelController.Lobby.IsReady = false;
            ModelController.Lobby.UpdateReadyStatus();

            Task.Run(() =>
            {
                for (int i = 10; i > 0; i--)
                {
                    if (CheckAbort())
                    {
                        ModelController.LoggerInstance.Write("Countdown aborted");
                        ModelController.SFXPlayer.Stop();
                        ModelController.Lobby.IsReady = true;
                        return;
                    }
                    if (i == 10)
                    {
                        ModelController.SFXPlayer.PlaySound(SFX.Race10);
                    }
                    ModelController.LoggerInstance.Write(i.ToString());
                    if (i == 3)
                    {
                        ModelController.SFXPlayer.PlaySound(SFX.Race321);
                        Client.HSync = new SyncHandler();
                    }
                    Task.Delay(1000).Wait();
                }
                ModelController.Lobby.IsReady = true;
                ModelController.LoggerInstance.Write("Go!");
            });

            
        }

        private static bool CheckAbort()
        {
            return !Client.HGameState.CheckMainMenu() || ModelController.Lobby.PlayerInfoList.Any(p => p.Level != "M/L");
        }
    }
}
