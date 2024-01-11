using System;

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
        var addr = _triggerAddress + index * 0xB8 + 0x8C;
        ProcessHandler.WriteData(addr, BitConverter.GetBytes(value), "TriggerHandler: SetTriggerActivity()");
    }

    public byte GetTriggerActivity(int index)
    {
        var addr = _triggerAddress + index * 0xB8 + 0x8C;
        ProcessHandler.TryRead(addr, out byte result, false, "TriggerHandler: GetTriggerActivity()");
        return result;
    }

    public void CheckSetTrigger(int index, bool value)
    {
        var b = value ? (byte)0x0 : (byte)0x1;
        if (GetTriggerActivity(index) == b)
            SetTriggerActivity(index, value);
    }
}