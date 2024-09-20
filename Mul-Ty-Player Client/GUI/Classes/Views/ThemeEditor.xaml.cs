using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Color = System.Windows.Media.Color;

namespace MulTyPlayerClient.GUI.Classes.Views;

public partial class ThemeEditor : Window
{
    public ThemeEditor()
    {
        InitializeComponent();
    }

    private void ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
    {
        if (e.NewValue.HasValue)
            App.AppColors.SetColor((sender as ColorPicker)!.Name, e.NewValue.Value);
    }
}