using MulTyPlayerClient;
using Riptide;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SyncHandler
    {
        IntPtr HProcess = ProcessHandler.HProcess;
        static LevelHandler HLevel => Program.HLevel;

        public static OpalHandler HOpal;

        public SyncHandler()
        {
            HOpal = new OpalHandler();
        }

        public void SendDataToServer(int index, int level, string type)
        {
            Console.WriteLine("sending to server");
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ServerDataUpdate);
            message.AddInt(index);
            message.AddInt(level);
            message.AddString(type);
            Client._client.Send(message);
        }

        public void SetMemAddrs()
        {
            HOpal.SetMemAddrs();
        }

        [MessageHandler((ushort)MessageID.ClientDataUpdate)]
        private static void HandleClientDataUpdate(Message message)
        {
            if(message.GetUShort() == Client._client.Id)  return;
            Console.WriteLine("handling data from server");
            int index = message.GetInt();
            int level = message.GetInt();
            string dataType = message.GetString();
            switch (dataType)
            {
                case "Opal": HOpal.HandleClientUpdate(index, level); break;
                default: { break; }
            }
        }
    }
}
