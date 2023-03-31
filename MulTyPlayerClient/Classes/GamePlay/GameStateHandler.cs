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
            ProcessHandler.TryRead(0x27EBCC, out byte result, true);
            return result == 0;
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
                }
            }
        }
    }
}
