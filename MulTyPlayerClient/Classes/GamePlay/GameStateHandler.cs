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
        public bool LoadingState { get; set; }
        public bool PreviousLoadingState { get; set; }

        public bool CheckMenuOrLoading()
        {
            ProcessHandler.TryRead(0x27EBCC, out byte result, true);
            return result == 0;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public bool CheckMainMenu()
        {
            ProcessHandler.TryRead(0x286641, out byte result, true);
            BasicIoC.MainGUIViewModel.IsOnMenu = result == 0;
            if(PlayerHandler.Players.TryGetValue(Client._client.Id, out Player value) && value.IsReady)
                value.IsReady = false;
            BasicIoC.MainGUIViewModel.UpdateReadyStatus();
            if (result == 0 && BasicIoC.MainGUIViewModel.PlayerInfoList?.Any(p => p.ClientID == Client._client.Id) == true)
            {
                BasicIoC.MainGUIViewModel.PlayerInfoList.First(p => p.ClientID == Client._client.Id).Level = "Menu";
            }
            return result == 0;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public void CheckLoaded()
        {
            PreviousLoadingState = LoadingState;
            LoadingState = CheckMenuOrLoading();
            CheckMainMenu();
            if (PreviousLoadingState != LoadingState)
            {
                PreviousLoadingState = LoadingState;
                if (!LoadingState)
                {
                    Client.HLevel.bNewLevelSetup = false;
                }
            }
        }
    }
}
