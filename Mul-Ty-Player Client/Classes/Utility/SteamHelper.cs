using System;
using Steamworks;

namespace MulTyPlayerClient.Classes.Utility;

internal static class SteamHelper
{
    public const int TY_APP_ID = 411960;

    public static bool Initialized => SteamClient.IsValid;

    public static void Init()
    {
        if (Initialized)
            return;
        try
        {
            SteamClient.Init(TY_APP_ID);
        }
        catch (Exception e)
        {
            Logger.Write("Error: SteamAPI initialization failed\n" + e);
        }
    }

    public static bool IsLoggedOn()
    {
        if (Initialized)
            return SteamClient.IsLoggedOn;
        return false;
    }

    public static string GetSteamName()
    {
        string userName = null;
        Init();

        if (IsLoggedOn()) userName = SteamClient.Name;
        return userName;
    }

    public static SteamId? GetSteamId()
    {
        SteamId? userId = null;
        Init();
        if (IsLoggedOn()) userId = SteamClient.SteamId;
        return userId;
    }
    
    public static void Shutdown()
    {
        SteamClient.Shutdown();
    }
}