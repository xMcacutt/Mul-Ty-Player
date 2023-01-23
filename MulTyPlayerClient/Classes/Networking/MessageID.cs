using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    public enum MessageID : ushort
    {
        Authentication,
        Connected,
        PlayerInfo,
        KoalaCoordinates,
        ConsoleSend,
        ServerDataUpdate,
        ClientDataUpdate,
        Disconnect,
        ResetSync,
        ReqSync,
        SyncSettings,
        ReqHost,
        HostChange,
        HostCommand,
        KoalaSelected
    }
}
