using MulTyPlayerClient.GUI;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MulTyPlayerClient
{
    public class GameStateHandler
    {
        public bool CurrentlyLoading;
        public bool WasLoadingLastFrame;

        public bool CheckMenuOrLoading()
        {
            ProcessHandler.TryRead(0x27EBCC, out bool result, true);
            return !result;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public bool CheckMainMenu()
        {
            ProcessHandler.TryRead(0x286641, out bool result, true);
            bool onMenu = !result;
            BasicIoC.MainGUIViewModel.IsOnMenu = onMenu;
            if(PlayerHandler.TryGetLocalPlayer(out Player player))
                player.IsReady &= onMenu;
            BasicIoC.MainGUIViewModel.UpdateReadyStatus();

            if (onMenu && BasicIoC.MainGUIViewModel.TryGetPlayerInfo(Client._client.Id, out PlayerInfo playerInfo))
            {
                playerInfo.Level = "M/L";
            }
            return onMenu;
        }

        public bool CheckLoaded()
        {
            CurrentlyLoading = CheckMenuOrLoading();
            CheckMainMenu();
            if (WasLoadingLastFrame && !CurrentlyLoading)
            {
                Client.HLevel.DoLevelSetup();
            }
            WasLoadingLastFrame = CurrentlyLoading;
            return CurrentlyLoading;
        }
    }
}
