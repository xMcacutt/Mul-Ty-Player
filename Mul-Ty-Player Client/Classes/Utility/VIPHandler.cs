using System.Collections.Generic;

namespace MulTyPlayerClient.Classes.Utility;

public class VIPHandler
{
    public static Dictionary<ulong, VIP> VIPs = new Dictionary<ulong, VIP>()
    {
        { 76561198040488721, VIP.Buzchy },
        { 76561198156210236, VIP.Matt },
        { 76561199048743874, VIP.Sirbeyy },
        { 76561198041462314, VIP.Kythol }
    };
    
    public static SFX GetSound(VIP vip)
    {
        switch (vip)
        {
            case VIP.Buzchy:
                return SFX.VIPJoinBuzchy;
            case VIP.Matt:
                return SFX.VIPJoinMatt;
            case VIP.Sirbeyy:
                return SFX.VIPJoinSirbeyy;
            case VIP.Kythol:
                return SFX.VIPJoinKythol;
        }
        return SFX.PlayerConnect;
    }
}

public enum VIP
{
    None,
    Buzchy,
    Sirbeyy,
    Matt,
    Kythol,
}