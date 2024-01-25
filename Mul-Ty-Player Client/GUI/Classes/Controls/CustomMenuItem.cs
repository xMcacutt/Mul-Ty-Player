using System.Windows;
using System.Windows.Controls;

namespace MulTyPlayerClient.GUI.Controls;

public class CustomMenuItem : MenuItem
{
    public static readonly DependencyProperty IconCodeProperty =
        DependencyProperty.Register(nameof(IconCode), typeof(string), typeof(CustomMenuItem), new PropertyMetadata(string.Empty));

    
    static CustomMenuItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomMenuItem), new FrameworkPropertyMetadata(typeof(CustomMenuItem)));
    }
    
    public string IconCode
    {
        get => (string)GetValue(IconCodeProperty); 
        set => SetValue(IconCodeProperty, value); 
    }
}