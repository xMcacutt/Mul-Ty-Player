using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.Classes.Views;

public partial class Minimap : Window
{
    public Minimap()
    {
        InitializeComponent();
    }

    public void MovePlayer1(float x1, float y1)
    {
        double degrees = 55;
        double radians = degrees * (double.Pi / 180d);
        double x2 = x1 * Math.Cos(radians) + y1 * Math.Sin(radians);
        double y2 = -x1 * Math.Sin(radians) + y1 * Math.Cos(radians);
        Canvas.SetLeft(Player1, (185 + x2 / 45f));
        Canvas.SetTop(Player1, 512 - (240 + y2 / 40f));
    }

    private void Minimap_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}