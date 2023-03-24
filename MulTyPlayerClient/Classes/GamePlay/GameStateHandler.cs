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
            return ProcessHandler.ReadData(PointerCalculations.AddOffset(0x25601C), 1, "Checking menu/loading")[0] == 0;
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
