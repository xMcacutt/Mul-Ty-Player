using System;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;

namespace MulTyPlayerClient.Classes.Utility;

public class HotkeyHandler
{
    public static void SetupHotkeys()
    {
        HotkeyManager.Current.AddOrReplace("groundswim", Key.G, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
        HotkeyManager.Current.AddOrReplace("ready", Key.R, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
        HotkeyManager.Current.AddOrReplace("requesthost", Key.H, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
        HotkeyManager.Current.AddOrReplace("start", Key.S, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
        HotkeyManager.Current.AddOrReplace("previous", Key.P, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
        HotkeyManager.Current.AddOrReplace("taunt", Key.T, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
        HotkeyManager.Current.AddOrReplace("crash", Key.C, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, OnKeyPress);
    }

    private static void OnKeyPress(object sender, HotkeyEventArgs e)
    {
        if (!Client.IsConnected)
            return;
        
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            Logger.Write("[ERROR] Failed to find self in player list.");
            return;
        }
        
        if (Client.HCommand.Commands.TryGetValue(e.Name, out var command))
        {
            if (command.HostOnly && !self.IsHost)
            {
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            }
            command.InitExecute(Array.Empty<string>());
            return;
        }
        
        if (e.Name == "previous" && Client.HCommand.Calls.TryPop(out var call))
        {
            Client.HCommand.ParseCommand(call); 
            return;
        }

        if (e.Name == "start")
        {
            if (!self.IsHost)
            {
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            }
            Client.HCommand.Commands["countdown"].InitExecute(new string[] {"start"});
            return;
        }
    }
}