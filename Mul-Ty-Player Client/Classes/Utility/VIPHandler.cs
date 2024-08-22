using System.Collections.Generic;

namespace MulTyPlayerClient.Classes.Utility;

public class VIPHandler
{
    public static Dictionary<ulong, VIP> VIPs = new Dictionary<ulong, VIP>()
    {
        { 76561198040488721, VIP.Buzchy },
        { 76561198156210236, VIP.Matt },
    };
    
    public static SFX GetSound(VIP vip)
    {
        switch (vip)
        {
            case VIP.Buzchy:
                return SFX.Alert;
            case VIP.Matt:
                return SFX.Alert;
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