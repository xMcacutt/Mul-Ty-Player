using System;
using System.Windows.Forms;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;

namespace MulTyPlayerClient.Classes.Utility;

public class Hotkeys
{
    public HotkeyConfig GroundSwim { get; set; }
    public HotkeyConfig Ready { get; set; }
    public HotkeyConfig RequestHost { get; set; }
    public HotkeyConfig Start { get; set; }
    public HotkeyConfig Previous { get; set; }
    public HotkeyConfig Taunt { get; set; }
    public HotkeyConfig RequestSync { get; set; }
    public HotkeyConfig Crash { get; set; }
    public HotkeyConfig CheatI { get; set; }
    public HotkeyConfig CheatT { get; set; }
    public HotkeyConfig CheatM { get; set; }
    public HotkeyConfig CheatE { get; set; }
    public HotkeyConfig CheatL { get; set; }
}

public class HotkeyConfig
{
    public Key Key { get; set; }
    public ModifierKeys Modifiers { get; set; }
}


public class HotkeyHandler
{
    
    
    
    private static Hotkeys GetDefaultHotkeys()
    {
        return new Hotkeys
        {
            GroundSwim = new HotkeyConfig { Key = Key.G, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            Ready = new HotkeyConfig { Key = Key.R, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            RequestHost = new HotkeyConfig { Key = Key.H, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            Start = new HotkeyConfig { Key = Key.S, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            Previous = new HotkeyConfig { Key = Key.P, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            Taunt = new HotkeyConfig { Key = Key.T, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            RequestSync = new HotkeyConfig { Key = Key.Q, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            Crash = new HotkeyConfig { Key = Key.C, Modifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt },
            CheatI = new HotkeyConfig { Key = Key.I, Modifiers = ModifierKeys.Alt | ModifierKeys.Shift },
            CheatT = new HotkeyConfig { Key = Key.T, Modifiers = ModifierKeys.Alt | ModifierKeys.Shift },
            CheatM = new HotkeyConfig { Key = Key.M, Modifiers = ModifierKeys.Alt | ModifierKeys.Shift },
            CheatE = new HotkeyConfig { Key = Key.E, Modifiers = ModifierKeys.Alt | ModifierKeys.Shift },
            CheatL = new HotkeyConfig { Key = Key.L, Modifiers = ModifierKeys.Alt | ModifierKeys.Shift },
        };
    }
    
    public static void SetupHotkeys(Hotkeys)
    {
#pragma warning disable CA1416
        try
        {
            HotkeyManager.Current.AddOrReplace("groundswim", Key.G, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("ready", Key.R, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("requesthost", Key.H, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("start", Key.S, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("previous", Key.P, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("taunt", Key.T, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("requestsync", Key.Q, ModifierKeys.Control | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("crash", Key.C, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_i", Key.I, ModifierKeys.Alt | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_t", Key.T, ModifierKeys.Alt | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_m", Key.M, ModifierKeys.Alt | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_e", Key.E, ModifierKeys.Alt | ModifierKeys.Shift, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_l", Key.L, ModifierKeys.Alt | ModifierKeys.Shift, OnKeyPress);
        }
        catch
        {
            MessageBox.Show(
                @"Global hotkeys may be disabled due to conflicts. Check running apps for conflicting global hotkeys.");
        }
#pragma warning restore CA1416
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

        if (e.Name == "requestsync")
        {
            Client.HSync?.RequestSync();
        }

        if (e.Name.StartsWith("cheat"))
        {
            Client.HCommand.Commands["cheat"].InitExecute(new [] {e.Name.Split('_')[1]});
        }
    }
}