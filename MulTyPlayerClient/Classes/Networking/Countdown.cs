using MulTyPlayerClient.GUI;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            BasicIoC.MainGUIViewModel.UpdateReadyStatus();
            Task.Run(() => {
                for (int i = 10; i > 0; i--)
                {
                    if (i == 10) { SFXPlayer.PlaySound(SFX.Race10); }
                    BasicIoC.LoggerInstance.Write(i.ToString());
                    if(i == 3) { SFXPlayer.PlaySound(SFX.Race321); }
                    Task.Delay(1000).Wait();
                }   
                BasicIoC.LoggerInstance.Write("Go!");
            });
        }
    }
}
