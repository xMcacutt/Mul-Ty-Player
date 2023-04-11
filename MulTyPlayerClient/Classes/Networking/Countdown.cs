﻿using MulTyPlayerClient.GUI;
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
            Task.Run(() => {
                MediaPlayer player = new() { Volume = 0.15 };
                for (int i = 10; i > 0; i--)
                {
                    if (CheckAbort())
                    {
                        BasicIoC.LoggerInstance.Write("Countdown aborted");
                        player.Stop();
                        BasicIoC.MainGUIViewModel.ReadyEnabled = true;
                        return;
                    }
                    if (i == 10) 
                    { 
                        player.Open(new Uri(SFX.Race10));
                        player.Play();
                    }
                    BasicIoC.LoggerInstance.Write(i.ToString());
                    if(i == 3) 
                    { 
                        player.Open(new Uri(SFX.Race321));
                        player.Play();
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
            return !Client.HGameState.CheckMainMenu();
        }
    }
}
