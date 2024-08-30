using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace MulTyPlayerClient;

public class MtpCommandRang : Command
{
    public MtpCommandRang()
    {
        Name = "rang";
        Aliases = new List<string> { "boomerang" };
        HostOnly = false;
        SpectatorAllowed = true;
        Usages = new List<string> { "/rang <rangName>" };
        Description = "Toggles a specific rang.";
        ArgDescriptions = new Dictionary<string, string>
        {
            {"<rangName>", "The name of the rang you want to toggle."},
        };   
    }

    private string[] rangs = new[]
    {
        "swim", "dive", "secondboomerang", "health", "boomerang", "frostyrang", "flamerang", "kaboomerang", "doomerang",
        "megarang", "zoomerang", "infrarang", "zappyrang", "aquarang", "multirang", "chronorang"
    };
    
    public override void InitExecute(string[] args)
    {
        if (args.Length != 1)
        {
            SuggestHelp();
            return;
        }
        
        if (!rangs.Any(x => x.StartsWith(args[0]))) 
            return;
        
        var byteIndex = Array.IndexOf(rangs, rangs.First(x => x.StartsWith(args[0])));
        RunRang(byteIndex);
    }

    public void RunRang(int byteIndex)
    {
        var addr = SyncHandler.SaveDataBaseAddress + 0xAA4 + byteIndex;
        ProcessHandler.TryRead(addr, out byte value, false, "Rang Command InitExecute()");
        var replacementValue = value == 0 ? (byte)1 : (byte)0;
        ProcessHandler.WriteData(addr, new byte[] { replacementValue });
        SFXPlayer.PlaySound(SFX.CheatActivated);
        Logger.Write($"Rang: {rangs[byteIndex]} toggled.");
    }
}