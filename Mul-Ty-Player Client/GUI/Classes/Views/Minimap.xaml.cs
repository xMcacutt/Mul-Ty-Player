using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MulTyPlayerClient.GUI.Classes.Views;

public partial class Minimap : Window
{
    public Minimap()
    {
        InitializeComponent();
    }

    public void MovePlayer1()
    {

        ProcessHandler.TryRead(0x28683C, out float x, true, "");
        ProcessHandler.TryRead(0x286844, out float z, true, "");
        
        ProcessHandler.TryRead(0x288E64, out float minX, true, "");
        ProcessHandler.TryRead(0x288E6C, out float minZ, true, "");
        ProcessHandler.TryRead(0x288E74, out float maxX, true, "");
        ProcessHandler.TryRead(0x288E7C, out float maxZ, true, "");
        
        var remap = Remap(x, z);
        var remapMin = Remap(minX, minZ);
        var remapMax = Remap(maxX, maxZ);
        
        // Normalize world coordinates to range [0, 1]
        var normX = (remap.X - remapMin.X) / (remapMax.X);  // Normalized X
        var normZ = (remap.Z - remapMin.Z) / (remapMax.Z);  // Normalized Z
        
        // Map to mini-map dimensions (assuming 512x512 mini-map)
        var mapX = normX * 512;  // Scale to mini-map width
        var mapZ = normZ * 512;  // Scale to mini-map height
        
        var yaw = -Client.HHero.GetCurrentPosRot()[4] * (180 / Math.PI) + 230;

        App.Current.Dispatcher.BeginInvoke(() =>
        {
            var rotateTransform = new RotateTransform(yaw);
            rotateTransform.CenterX = Player1.Width / 2;
            rotateTransform.CenterY = Player1.Height / 2;
            Player1.RenderTransform = rotateTransform;
            Canvas.SetLeft(Player1, mapX);
            Canvas.SetTop(Player1, mapZ);
            //Console.WriteLine($"CenterX: {rotateTransform.CenterX}, CenterY: {rotateTransform.CenterY}");
        });

        if (count == 100)
        {
            Console.WriteLine(mapX + ", " + mapZ);
            count = 0;
        }
        count++;
    }

    public static int count = 0;

    private MapCoords Remap(float xWorld, float zWorld)
    {
        var t = new float[16];
        for (var i = 0; i < 16; i++)
            ProcessHandler.TryRead(0x28685C + 4 * i, out t[i], true, "");
        var x = t[0] * xWorld + t[8] * zWorld + t[12];
        var z = t[2] * xWorld + t[10] * zWorld + t[14];
        return new MapCoords(x, z);
    }
    
    private void Minimap_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}

struct MapCoords
{
    public float X;
    public float Z;

    public MapCoords(float x, float z)
    {
        X = x;
        Z = z;
    }

    public static MapCoords operator +(MapCoords c1, MapCoords c2)
    {
        return new MapCoords(c1.X + c2.X, c1.Z + c2.Z);
    }
}