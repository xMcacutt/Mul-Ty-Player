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
        const int degrees = 55;
        const double radians = degrees * (double.Pi / 180d);
        const double offsetX = 185d;
        const double offsetY = 240d;
        const double scaleX = 0.45d;
        const double scaleY = 0.4d;
        var x2 = x1 * Math.Cos(radians) + y1 * Math.Sin(radians);
        var y2 = -x1 * Math.Sin(radians) + y1 * Math.Cos(radians);
        Canvas.SetLeft(Player1, (offsetX + x2) * scaleX);
        Canvas.SetTop(Player1, 512 - (offsetY + y2) * scaleY);
    }

    private void Minimap_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}