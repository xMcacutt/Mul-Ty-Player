using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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
    public bool UseSplash { get; set; }
    public Color MainBackColor {get; set;}
    public Color AltBackColor {get; set;}
    public Color SpecialBackColor {get; set;}
    public Color MainTextColor {get; set;}
    public Color AltTextColor {get; set;}
    public Color InvertedTextColor {get; set;}
    public Color MainAccentColor {get; set;}
    public Color AltAccentColor {get; set;}
    
    private Dictionary<string, Action<Color>> colorSetters;
    public Colors()
    {
        MainBack = new SolidColorBrush();
        AltBack = new SolidColorBrush();
        SpecialBack = new SolidColorBrush();
        MainText = new SolidColorBrush();
        AltText = new SolidColorBrush();
        InvertedText = new SolidColorBrush();
        MainAccent = new SolidColorBrush();
        AltAccent = new SolidColorBrush();
        colorSetters = new Dictionary<string, Action<Color>>
        {
            { "MainBack", color => { MainBackColor = color; MainBack.Color = color; } },
            { "AltBack", color => { AltBackColor = color; AltBack.Color = color; } },
            { "SpecialBack", color => { SpecialBackColor = color; SpecialBack.Color = color; } },
            { "MainText", color => { MainTextColor = color; MainText.Color = color; } },
            { "AltText", color => { AltTextColor = color; AltText.Color = color; } },
            { "InvertedText", color => { InvertedTextColor = color; InvertedText.Color = color; } },
            { "MainAccent", color => { MainAccentColor = color; MainAccent.Color = color; } },
            { "AltAccent", color => { AltAccentColor = color; AltAccent.Color = color; } }
        };
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
        UseSplash = scheme.UseSplash;
        
        MainBack.Color = MainBackColor;
        AltBack.Color = AltBackColor;
        SpecialBack.Color = SpecialBackColor;
        MainText.Color = MainTextColor;
        AltText.Color = AltTextColor;
        InvertedText.Color = InvertedTextColor;
        MainAccent.Color = MainAccentColor;
        AltAccent.Color = AltAccentColor;
    }

    public void UpdateColors()
    {
        MainBack.Color = MainBackColor;
        AltBack.Color = AltBackColor;
        SpecialBack.Color = SpecialBackColor;
        MainText.Color = MainTextColor;
        AltText.Color = AltTextColor;
        InvertedText.Color = InvertedTextColor;
        MainAccent.Color = MainAccentColor;
        AltAccent.Color = AltAccentColor;
    }

    public void SetColor(string name, Color color)
    {
        if (colorSetters.TryGetValue(name, out var action))
            action(color);
    }
}