using System;
using System.Collections.Generic;
using MulTyPlayer;
using MulTyPlayerClient;
using MulTyPlayerClient.Classes.Networking;
using Riptide;
using Client = Riptide.Client;

namespace MulTyPlayerClient;

public class GlowHandler
{
    public Dictionary<int, GlowColor> GlowColors = new()
    {
        { (int)Koala.Katie, new GlowColor(3.5f, 1.25f, 1f) },
        { (int)Koala.Mim, new GlowColor(2.5f, 2.5f, 3.5f) },
        { (int)Koala.Elizabeth, new GlowColor(3f, 0f, 4f) },
        { (int)Koala.Snugs, new GlowColor(5f, 2.5f, 0f) },
        { (int)Koala.Dubbo, new GlowColor(1f, 1f, 5f) },
        { (int)Koala.Gummy, new GlowColor(5f, 0f, 0f) },
        { (int)Koala.Kiki, new GlowColor(0.5f, 5f, 0.5f) },
        { (int)Koala.Boonie, new GlowColor(5f, 1f, 0f) }
    };
    
    
    private static bool readyToWriteTransformData;
    private static int _baseGlowAddress;

    public GlowHandler()
    {
        CreateGlowAddressArray();
    }

    private static GameStateHandler HGameState => Client.HGameState;
    private static LevelHandler HLevel => Client.HLevel;
    public static GlowTransformAddresses[] TransformAddresses { get; private set; }

    public void CreateGlowAddressArray()
    {
        TransformAddresses = new GlowTransformAddresses[8];
        for (var i = 0; i < 8; i++) TransformAddresses[i] = new GlowTransformAddresses();
    }

    public void SetBaseAddress()
    {
        _baseGlowAddress = PointerCalculations.GetPointerAddress(0x274B90, new[] { 0x15C, 0x74, 0x0 });
        ProcessHandler.CheckAddress(_baseGlowAddress, (ushort)27896, "Glow base address check");
    }

    public void SetCoordinateAddresses()
    {
        readyToWriteTransformData = false;
        if (_baseGlowAddress == 0)
            SetBaseAddress();
        for (var koalaId = 0; koalaId < 8; koalaId++)
        {
            var glowDescOffset = 0xAC * koalaId;
            ProcessHandler.TryRead(_baseGlowAddress + glowDescOffset + 0x8, out int glowObjectAddress, false, "Glow address set"); 
            TransformAddresses[koalaId] = new GlowTransformAddresses
            {
                X = glowObjectAddress + 0x74,
                Y = glowObjectAddress + 0x78,
                Z = glowObjectAddress + 0x7C,
                sX = glowObjectAddress + 0x44,
                sY = glowObjectAddress + 0x58,
                sZ = glowObjectAddress + 0x6C,
                R = glowObjectAddress + 0x34,
                G = glowObjectAddress + 0x38,
                B = glowObjectAddress + 0x3C
            };
        }
        
        SetColors();

        readyToWriteTransformData = true;
    }
    
    public void SetColors()
    {
        for (var i = 0; i < 8; i++)
        {
            var color = GlowColors[i];
            ProcessHandler.WriteData(TransformAddresses[i].R, BitConverter.GetBytes(color.R));
            ProcessHandler.WriteData(TransformAddresses[i].G, BitConverter.GetBytes(color.G));
            ProcessHandler.WriteData(TransformAddresses[i].B, BitConverter.GetBytes(color.B));
        }
    }

    public void ReturnGlows()
    {
        for (var i = 0; i < 8; i++)
            ReturnGlow(i);
    }

    public void ReturnGlow(int koalaId)
    {
        ProcessHandler.WriteData(TransformAddresses[koalaId].sX, BitConverter.GetBytes(2500f), "Scaling glows");
        ProcessHandler.WriteData(TransformAddresses[koalaId].sY, BitConverter.GetBytes(2500f), "Scaling glows");
        ProcessHandler.WriteData(TransformAddresses[koalaId].sZ, BitConverter.GetBytes(2500f), "Scaling glows");
    }

    public struct GlowTransformAddresses
    {
        public int X, Y, Z, sX, sY, sZ, R, G, B;
    }
}