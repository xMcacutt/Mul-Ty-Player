using MulTyPlayerClient.GUI;
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
            BasicIoC.MainGUIViewModel.ReadyEnabled = false;
            BasicIoC.MainGUIViewModel.UpdateReadyStatus();

            Task.Run(() =>
            {
                for (int i = 10; i > 0; i--)
                {
                    if (CheckAbort())
                    {
                        BasicIoC.LoggerInstance.Write("Countdown aborted");
                        BasicIoC.SFXPlayer.Stop();
                        BasicIoC.MainGUIViewModel.ReadyEnabled = true;
                        return;
                    }
                    if (i == 10)
                    {
                        BasicIoC.SFXPlayer.PlaySound(SFX.Race10);
                    }
                    BasicIoC.LoggerInstance.Write(i.ToString());
                    if (i == 3)
                    {
                        BasicIoC.SFXPlayer.PlaySound(SFX.Race321);
                        Client.HSync = new SyncHandler();
                    }
                    Task.Delay(1000).Wait();
                }
                BasicIoC.MainGUIViewModel.ReadyEnabled = true;
                BasicIoC.LoggerInstance.Write("Go!");
            });

            
        }

        private static bool CheckAbort()
        {
            return !Client.HGameState.CheckMainMenu() || BasicIoC.MainGUIViewModel.PlayerInfoList.Any(p => p.Level != "M/L");
        }
    }
}
