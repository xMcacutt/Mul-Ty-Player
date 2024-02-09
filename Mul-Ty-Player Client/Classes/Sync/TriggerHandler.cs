using System;
using System.Linq;

namespace MulTyPlayerClient;

public class TriggerHandler
{
    private int _triggerAddress;

    public TriggerHandler()
    {
        SetMemAddrs();
    }

    public void SetMemAddrs()
    {
        _triggerAddress = PointerCalculations.GetPointerAddress(0x26DBD8, new int[] { 0x0 });
    }

    public void SetTriggerActivity(int index, bool value)
    {
        var addr = _triggerAddress + index * 0xB8 + 0x88;
        ProcessHandler.WriteData(addr, Enumerable.Repeat(BitConverter.GetBytes(value)[0], 3).ToArray(), "TriggerHandler: SetTriggerActivity()");
        ProcessHandler.WriteData(addr + 4, BitConverter.GetBytes(value), "TriggerHandler: SetTriggerActivity()");
    }

    public byte GetTriggerActivity(int index)
    {
        var addr = _triggerAddress + index * 0xB8 + 0x8C;
        ProcessHandler.TryRead(addr, out byte result, false, "TriggerHandler: GetTriggerActivity()");
        return result;
    }

    public void CheckSetTrigger(int index, bool value)
    {
        Console.WriteLine($"{index} {value}");
        var b = value ? (byte)0x0 : (byte)0x1;
        if (GetTriggerActivity(index) == b)
            SetTriggerActivity(index, value);
    }
}