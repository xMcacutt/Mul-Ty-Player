using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    public class GameStateHandler
    {
        public bool LoadingState { get; set; }
        public bool PreviousLoadingState { get; set; }

        public bool CheckMenuOrLoading()
        {
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), PointerCalculations.AddOffset(0x25601C), buffer, 1, ref bytesRead);
            return buffer[0] == 0;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public void CheckLoaded()
        {
            PreviousLoadingState = LoadingState;
            LoadingState = CheckMenuOrLoading();
            if (PreviousLoadingState != LoadingState)
            {
                PreviousLoadingState = LoadingState;
                if (!LoadingState)
                {
                    Client.HLevel.bNewLevelSetup = false;
                    Client.HLevel.LoadedNewLevelNetworkingSetupDone = false;
                }
            }
        }
    }
}
