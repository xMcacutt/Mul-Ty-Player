using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MulTyPlayerClient.GUI.Controls;

public partial class KoalaIcon : UserControl
{
    public static readonly DependencyProperty GradientStartColorProperty =
        DependencyProperty.Register(nameof(GradientStartColor), typeof(Color), typeof(KoalaIcon),
            new PropertyMetadata(System.Windows.Media.Colors.White));

    public static readonly DependencyProperty GradientEndColorProperty =
        DependencyProperty.Register(nameof(GradientEndColor), typeof(Color), typeof(KoalaIcon),
            new PropertyMetadata(System.Windows.Media.Colors.White));

    public static readonly DependencyProperty LetterProperty =
        DependencyProperty.Register(nameof(Letter), typeof(string), typeof(Icon), new PropertyMetadata("X"));

    public KoalaIcon()
    {
        InitializeComponent();
    }

    public string Letter
    {
        get => (string)GetValue(LetterProperty);
        set => SetValue(LetterProperty, value);
    }

    public Color GradientStartColor
    {
        get => (Color)GetValue(GradientStartColorProperty);
        set => SetValue(GradientStartColorProperty, value);
    }

    public Color GradientEndColor
    {
        get => (Color)GetValue(GradientEndColorProperty);
        set => SetValue(GradientEndColorProperty, value);
    }
}