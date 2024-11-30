using System;
using System.Drawing;
using System.Linq;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Utility;
using Riptide;

namespace MulTyPlayerClient.Classes.GamePlay;

public enum HDC_IcicleBehaviour
{
    BlackIce,
    MemoryGame,
    DeathByIcicle,
    ColorSwap
}

public class HDC_IcicleHandler
{
    private static bool _setup;
    private static Random _random = new Random();
    
    public static void HandleIcicles(int count, int baseAddr, HDC_IcicleBehaviour behaviour)
    {
        switch (behaviour)
        {
            case HDC_IcicleBehaviour.BlackIce: 
                RunBlackIce(count, baseAddr);
                break;
            case HDC_IcicleBehaviour.DeathByIcicle: 
                RunDeathByIcicle();
                break;
            case HDC_IcicleBehaviour.ColorSwap: 
                RunColorSwap(count, baseAddr);
                break;
            case HDC_IcicleBehaviour.MemoryGame: 
                RunMemoryGame(count, baseAddr);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
        }
        
    }

    public static void ResetIcicles()
    {
        _setup = false;
    }

    private static void RunBlackIce(int count, int baseAddr)
    {
        if (_setup)
            return;
        var blackColor = new RGB(0, 0, 0);
        var currentAddr = baseAddr;
        for (var icicleIndex = 0; icicleIndex < count; icicleIndex++)
        {
            ProcessHandler.TryRead(currentAddr + 0x8, out int deeperAddr, false, "blackIce");
            ProcessHandler.WriteData(deeperAddr + 0x34, blackColor.GetBytes());
            ProcessHandler.TryRead(currentAddr + 0x34, out currentAddr, false, "blackIce2");
        }
        _setup = true;
    }

    private static bool _colorSet;
    private static void RunMemoryGame(int count, int baseAddr)
    {
        var hitCountAddr = PointerCalculations.GetPointerAddress(0x26BA50, new[] { 0xA0 });
        ProcessHandler.TryRead(hitCountAddr, out int hitCount, false, "memoryGame");
        if (hitCount != 0)
        {
            LevelHandler.LevelBloomSettings.State = true;
            LevelHandler.LevelBloomSettings.Saturation = 0;
        }
        else 
            LevelHandler.LevelBloomSettings.RevertToOriginal();
    }

    private static bool _isActive;
    private static void RunDeathByIcicle()
    {
        var hitCountAddr = PointerCalculations.GetPointerAddress(0x26BA50, new[] { 0xA0 });
        ProcessHandler.TryRead(hitCountAddr, out int hitCount, false, "memoryGame");
        if (hitCount > 0)
            _isActive = true;
        if (!_isActive || hitCount != 0) 
            return;
        Client.HHardcore.EndRun();
        _isActive = false;
    }

    private static void RunColorSwap(int count, int baseAddr)
    {
        if (_setup)
            return;
        var currentAddr = baseAddr;
        for (var icicleIndex = 0; icicleIndex < count; icicleIndex++)
        {
            var color = new RGB(_random.NextSingle(), _random.NextSingle(), _random.NextSingle());
            ProcessHandler.TryRead(currentAddr + 0x8, out int deeperAddr, false, "colorSwap");
            ProcessHandler.WriteData(deeperAddr + 0x34, color.GetBytes());
            ProcessHandler.TryRead(currentAddr + 0x34, out currentAddr, false, "colorSwap2");
        }
        _setup = true;
    }
}