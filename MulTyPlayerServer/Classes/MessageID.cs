using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    public enum MessageID : ushort
    {
        Connected,
        PlayerInfo,
        KoalaCoordinates,
        ConsoleSend,
        ServerDataUpdate,
        ClientLevelDataUpdate,
        ClientAttributeDataUpdate,
        Disconnect,
        ResetSync,
        ReqHost,
        HostChange,
        HostCommand,
        ReqSync,
        OpalCollected
    }
}
