using Riptide;
using System;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    public class HeroHandler
    {
        static LevelHandler HLevel => Client.HLevel;
        static GameStateHandler HGameState => Client.HGameState;

        const int TY_POSROT_BASE_ADDRESS = 0x270B78;
        const int BP_POSROT_BASE_ADDRESS = 0x254268;
        float[] CurrentPosRot;

        public HeroHandler()
        {
            CurrentPosRot = new float[6];
        }

        public void GetTyPosRot()
        {
            //GETS TY'S OR BUSHPIG'S POSITION AND ROTATION AND STORES IT IN CURRENTPOSROT 
            int baseAddress = HLevel.CurrentLevelId == 10 ? BP_POSROT_BASE_ADDRESS : TY_POSROT_BASE_ADDRESS;
            int rotOffset = HLevel.CurrentLevelId == 10 ? 0x388 : 0x10A4;

            for (int i = 0; i < 6; i++)
            {
                int offset = i < 3 ? i * 0x4 : rotOffset + (i - 3) * 0x4;
                ProcessHandler.TryRead(baseAddress + offset, out CurrentPosRot[i], true);
            }
        }

        public void SendCoordinates()
        {
            //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
            Message message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
            message.AddBool(HGameState.CheckMainMenu());
            message.AddInt(HLevel.CurrentLevelId);
            message.AddFloats(Client.HHero.CurrentPosRot);
            Client._client.Send(message);
        }
    }

}
