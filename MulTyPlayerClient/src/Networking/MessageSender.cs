using MulTyPlayerCommon;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.Networking
{
    internal class MessageSender
    {
        public static void SendPlayerCoordinates(float[] position, float[] rotation)
        {
            //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
            Message message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);

            message.AddBool(Replication.HGameState.CheckMainMenu());
            message.AddInt(LevelHandler.CurrentLevelId);

            message.AddFloats(position);
            message.AddFloats(rotation);

            ConnectionService.Client.Send(message);
        }
    }
}
