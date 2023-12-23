using MulTyPlayerClient.GUI.Views;

namespace MulTyPlayerClient.GUI;

internal class WindowHandler
{
    public static SettingsMenu SettingsWindow;
    public static Setup SetupWindow;

    public static void InitializeWindows()
    {
        SetupWindow = new Setup();
    }
}