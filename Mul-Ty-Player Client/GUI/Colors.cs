using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using MulTyPlayerClient.GUI.Themes;
using Newtonsoft.Json;
using PropertyChanged;

namespace MulTyPlayerClient.GUI;

[AddINotifyPropertyChangedInterface]
public class Colors
{
    public SolidColorBrush MainBack { get; set; }
    public SolidColorBrush AltBack {get; set;}
    public SolidColorBrush SpecialBack {get; set;}
    public SolidColorBrush MainText {get; set;}
    public SolidColorBrush AltText {get; set;}
    public SolidColorBrush InvertedText {get; set;}
    public SolidColorBrush MainAccent {get; set;}
    public SolidColorBrush AltAccent {get; set;}
    public Color MainBackColor {get; set;}
    public Color AltBackColor {get; set;}
    public Color SpecialBackColor {get; set;}
    public Color MainTextColor {get; set;}
    public Color AltTextColor {get; set;}
    public Color InvertedTextColor {get; set;}
    public Color MainAccentColor {get; set;}
    public Color AltAccentColor {get; set;}
        
    public Colors()
    {
        SetColors("Dark");
    }

    public void SetColors(string theme)
    {
        var json = File.ReadAllText($"GUI/Themes/{theme}.json");
        var scheme = JsonConvert.DeserializeObject<ColorScheme>(json);
        MainBackColor = (Color)ColorConverter.ConvertFromString(scheme.MainBackColor)!;
        AltBackColor = (Color)ColorConverter.ConvertFromString(scheme.AltBackColor)!;
        SpecialBackColor = (Color)ColorConverter.ConvertFromString(scheme.SpecialBackColor)!;
        MainTextColor = (Color)ColorConverter.ConvertFromString(scheme.MainTextColor)!;
        AltTextColor = (Color)ColorConverter.ConvertFromString(scheme.AltTextColor)!;
        InvertedTextColor = (Color)ColorConverter.ConvertFromString(scheme.InvertedTextColor)!;
        MainAccentColor = (Color)ColorConverter.ConvertFromString(scheme.MainAccentColor)!;
        AltAccentColor = (Color)ColorConverter.ConvertFromString(scheme.AltAccentColor)!;
        
        MainBack = new SolidColorBrush(MainBackColor);
        AltBack = new SolidColorBrush(AltBackColor);
        SpecialBack = new SolidColorBrush(SpecialBackColor);
        MainText = new SolidColorBrush(MainTextColor);
        AltText = new SolidColorBrush(AltTextColor);
        InvertedText = new SolidColorBrush(InvertedTextColor);
        MainAccent = new SolidColorBrush(MainAccentColor);
        AltAccent = new SolidColorBrush(AltAccentColor);
    }
}