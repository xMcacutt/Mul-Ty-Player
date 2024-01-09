using System.Collections.Generic;
using System.Linq;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Sync;

internal class PortalSyncer : Syncer
{
    public static int[] FlakyPortals = { 4, 7, 15, 19, 21, 22, 23 };
    public static Dictionary<int, byte> ActivePortals;

    public PortalSyncer()
    {
        Name = "Portal";
        ActivePortals = new Dictionary<int, byte>();
        foreach (var i in FlakyPortals) ActivePortals.Add(i, 0);
    }

    public override void HandleServerUpdate(int null1, int null2, int level, ushort originalSender)
    {
        //Console.WriteLine("Portal for level " + level + " open");
        if (ActivePortals[level] == 1) return;
        ActivePortals[level] = 1;
        SendUpdatedData(null1, null2, level, originalSender);
    }

    public override void Sync(ushort player)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);
        message.AddString(Name);
        message.AddInt(0);
        message.AddBytes(ActivePortals.Values.ToArray());
        message.AddBytes(new byte[] { 0 });
        Server._Server.Send(message, player);
    }
}