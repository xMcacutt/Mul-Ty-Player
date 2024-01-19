using System.Windows;
using System.Windows.Controls;

namespace MulTyPlayerClient.GUI.Controls;

public class CustomMenuItem : MenuItem
{
    public static readonly DependencyProperty IconCodeProperty =
        DependencyProperty.Register(nameof(IconCode), typeof(string), typeof(CustomMenuItem));

    public string IconCode
    {
        get { return (string)GetValue(IconCodeProperty); }
        set { SetValue(IconCodeProperty, value); }
    }
}