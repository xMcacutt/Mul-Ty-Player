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
        SetColors(false);
    }

    public void SetColors(bool darkTheme)
    {
        Clear();
        
        MainBackColor = darkTheme ? 
            Color.FromArgb(0xff,0x2b,0x27,0x27) : Color.FromArgb(0xff,0xd5,0xdd,0xe6);
        
        AltBackColor = darkTheme ? 
            Color.FromArgb(0xff,0x3f,0x3b,0x3b) : Color.FromArgb(0xff,0xda,0xe1,0xe9);
        
        SpecialBackColor = darkTheme ? 
            Color.FromArgb(0xff,0x54,0x51,0x51) : Color.FromArgb(0xff,0xde,0xe4,0xec);
        
        MainTextColor = darkTheme ? 
            Color.FromArgb(0xff,0xff,0xff,0xff) : Color.FromArgb(0xff,0x00,0x00,0x00);
        
        AltTextColor = darkTheme ? 
            Color.FromArgb(0xff,0x99,0x99,0x99) : Color.FromArgb(0xff,0x44,0x44,0x44);
        
        InvertedTextColor = darkTheme ? 
            Color.FromArgb(0xff,0x00,0x00,0x00) : Color.FromArgb(0xff,0xff,0xff,0xff);
        
        MainAccentColor = darkTheme ? 
            Color.FromArgb(0xff,0xe8,0x9f,0x75) : Color.FromArgb(0xff,0xd8,0x79,0x41);
        
        AltAccentColor = darkTheme ? 
            Color.FromArgb(0xff,0xd8,0x79,0x41) : Color.FromArgb(0xff,0xba,0x72,0x06);
        
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