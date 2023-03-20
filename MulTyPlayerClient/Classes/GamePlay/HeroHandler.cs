using Riptide;
using System;

namespace MulTyPlayerClient
{
    public class HeroHandler
    {
        static LevelHandler HLevel => Client.HLevel;
        static GameStateHandler HGameState => Client.HGameState;

        const int TY_POSROT_BASE_ADDRESS = 0x270B78;
        const int BP_POSROT_BASE_ADDRESS = 0x254268;
        int[] _tyPosRotAddrs;
        int[] _bpPosRotAddrs;
        float[] CurrentPosRot;

        public HeroHandler()
        {
            CurrentPosRot = new float[6];
            _tyPosRotAddrs = new int[6];
            _bpPosRotAddrs = new int[6];
        }

        public void SetCoordAddrs()
        {
            //SET ADDRESSES RELATIVE TO ADDRESS OF X COORDINATE
            _tyPosRotAddrs[0] = PointerCalculations.AddOffset(TY_POSROT_BASE_ADDRESS);
            _tyPosRotAddrs[1] = _tyPosRotAddrs[0] + 0x4;
            _tyPosRotAddrs[2] = _tyPosRotAddrs[1] + 0x4;
            _tyPosRotAddrs[3] = _tyPosRotAddrs[2] + 0x109C;
            _tyPosRotAddrs[4] = _tyPosRotAddrs[3] + 0x4;
            _tyPosRotAddrs[5] = _tyPosRotAddrs[4] + 0x4;
            _bpPosRotAddrs[0] = PointerCalculations.AddOffset(BP_POSROT_BASE_ADDRESS);
            _bpPosRotAddrs[1] = _bpPosRotAddrs[0] + 0x4;
            _bpPosRotAddrs[2] = _bpPosRotAddrs[1] + 0x4;
            _bpPosRotAddrs[3] = _bpPosRotAddrs[2] + 0x380;
            _bpPosRotAddrs[4] = _bpPosRotAddrs[3] + 0x4;
            _bpPosRotAddrs[5] = _bpPosRotAddrs[4] + 0x4;
        }

        public async void GetTyPosRot()
        {
            //GETS TY'S OR BUSHPIG'S POSITION AND ROTATION AND STORES IT IN CURRENTPOSROT 
            int[] tempInts = HLevel.CurrentLevelId == 10 ? _bpPosRotAddrs : _tyPosRotAddrs;
            for (int i = 0; i < tempInts.Length; i++)
            {
                CurrentPosRot[i] = BitConverter.ToSingle(await ProcessHandler.ReadDataAsync(tempInts[i], 4), 0);
            }
        }

        public async void SendCoordinates()
        {
            //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
            Message message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
            message.AddBool(await HGameState.CheckMenuOrLoading());
            message.AddInt(HLevel.CurrentLevelId);
            message.AddFloats(Client.HHero.CurrentPosRot);
            Client._client.Send(message);
        }
    }

}
