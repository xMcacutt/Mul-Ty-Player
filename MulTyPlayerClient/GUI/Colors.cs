namespace MulTyPlayerClient.GUI;

using System.Windows;
using System.Windows.Media;

public class Colors : ResourceDictionary
{
    public static Color MainBackColor;
    public static Color AltBackColor;
    public static Color SpecialBackColor;
    public static Color MainTextColor;
    public static Color AltTextColor;
    public static Color InvertedTextColor;
    public static Color MainAccentColor;
    public static Color AltAccentColor;
    
    public Colors()
    {
        SetColors(true);
    }

    public void SetColors(bool darkTheme)
    {
        Clear();
        
        MainBackColor = darkTheme ? 
            Color.FromArgb(0xff,0x21,0x24,0x26) : Color.FromArgb(0xff,0xee,0xeb,0xe9);
        
        AltBackColor = darkTheme ? 
            Color.FromArgb(0xff,0x34,0x36,0x38) : Color.FromArgb(0xff,0xd6,0xd9,0xdc);
        
        SpecialBackColor = darkTheme ? 
            Color.FromArgb(0xff,0x4C,0x4E,0x50) : Color.FromArgb(0xff,0xcf, 0xcf, 0xcf);
        
        MainTextColor = darkTheme ? 
            Color.FromArgb(0xff,0xff,0xff,0xff) : Color.FromArgb(0xff,0x00,0x00,0x00);
        
        AltTextColor = darkTheme ? 
            Color.FromArgb(0xff,0x99,0x99,0x99) : Color.FromArgb(0xff,0x44,0x44,0x44);
        
        InvertedTextColor = darkTheme ? 
            Color.FromArgb(0xff,0x00,0x00,0x00) : Color.FromArgb(0xff,0xff,0xff,0xff);
        
        MainAccentColor = darkTheme ? 
            Color.FromArgb(0xff,0xe7,0x99,0x41) : Color.FromArgb(0xff,0x18, 0x66, 0xbe);
        
        AltAccentColor = darkTheme ? 
            Color.FromArgb(0xff,0x29, 0x60, 0x9f) : Color.FromArgb(0xff,0xf7,0xa9,0x71);
        
        AddColors();
    }

    private void AddColors()
    {
        Add("MainBack", MainBackColor);
        Add("AltBack", AltBackColor);
        Add("SpecialBack", SpecialBackColor);
        Add("MainText", MainTextColor);
        Add("AltText", AltTextColor);
        Add("InvertedText", InvertedTextColor);
        Add("MainAccent", MainAccentColor);
        Add("AltAccent", AltAccentColor);
    }
    
    private void Add(string key, object value)
    {
        this[key] = new SolidColorBrush((Color)value);
        this[key + "Color"] = value;
    }
}