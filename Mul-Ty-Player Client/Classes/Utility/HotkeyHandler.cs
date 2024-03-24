using System;
using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Windows.Forms;
using System.Windows.Input;
using MulTyPlayerClient.GUI.Models;
using Newtonsoft.Json;
using NHotkey;
using NHotkey.Wpf;

namespace MulTyPlayerClient.Classes.Utility;

public class Hotkeys
{
    public HotkeyConfig Freecam { get; set; }
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
    public HotkeyConfig SpectatorFreecamPrevious { get; set; }
    public HotkeyConfig SpectatorFreecamNext { get; set; }
    public HotkeyConfig SpectatorFreecamDisengage { get; set; }
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
            Freecam = new HotkeyConfig { Key = Key.F, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
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
            SpectatorFreecamPrevious = new HotkeyConfig { Key = Key.D9, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            SpectatorFreecamNext = new HotkeyConfig { Key = Key.D0, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
            SpectatorFreecamDisengage = new HotkeyConfig { Key = Key.D8, Modifiers = ModifierKeys.Control | ModifierKeys.Shift }
        };
    }
    
    public static void SetupHotkeys(Hotkeys keys)
    {
        #pragma warning disable CA1416
        try
        {
            HotkeyManager.Current.AddOrReplace("freecam", keys.Freecam.Key, keys.Freecam.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("groundswim", keys.GroundSwim.Key, keys.GroundSwim.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("ready", keys.Ready.Key, keys.Ready.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("requesthost", keys.RequestHost.Key, keys.RequestHost.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("start", keys.Start.Key, keys.Start.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("previous", keys.Previous.Key, keys.Previous.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("taunt", keys.Taunt.Key, keys.Taunt.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("requestsync", keys.RequestSync.Key, keys.RequestSync.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("crash", keys.Crash.Key, keys.Crash.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_i", keys.CheatI.Key, keys.CheatI.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_t", keys.CheatT.Key, keys.CheatT.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_m", keys.CheatM.Key, keys.CheatM.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_e", keys.CheatE.Key, keys.CheatE.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("cheat_l", keys.CheatL.Key, keys.CheatL.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("spectator_freecam_previous", keys.SpectatorFreecamPrevious.Key, keys.SpectatorFreecamPrevious.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("spectator_freecam_next", keys.SpectatorFreecamNext.Key, keys.SpectatorFreecamNext.Modifiers, OnKeyPress);
            HotkeyManager.Current.AddOrReplace("spectator_freecam_disengage", keys.SpectatorFreecamDisengage.Key, keys.SpectatorFreecamDisengage.Modifiers, OnKeyPress);
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
            if (command.HostOnly && !self.IsHost || !command.SpectatorAllowed && ModelController.Login.JoinAsSpectator)
            {
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            }
            command.InitExecute(Array.Empty<string>());
            return;
        }
        switch (e.Name)
        {
            case "previous" when Client.HCommand.Calls.TryPop(out var call):
                Client.HCommand.ParseCommand(call); 
                return;
            case "start" when !self.IsHost:
                Logger.Write("[ERROR] You do not have the privileges to run this command.");
                return;
            case "start":
                Client.HCommand.Commands["countdown"].InitExecute(new string[] {"start"});
                return;
            case "spectator_freecam_previous":
                if (ModelController.Login.JoinAsSpectator)
                    SpectatorHandler.FindPreviousSpectatee();
                return;
            case "spectator_freecam_next":
                if (ModelController.Login.JoinAsSpectator)
                    SpectatorHandler.FindNextSpectatee();
                return;
            case "spectator_freecam_disengage":
                if (ModelController.Login.JoinAsSpectator)
                    SpectatorHandler.SpectateeKoalaId = null;
                return;
            case "requestsync":
                Client.HSync?.RequestSync();
                return;
        }

        if (e.Name.StartsWith("cheat"))
        {
            Client.HCommand.Commands["cheat"].InitExecute(new [] {e.Name.Split('_')[1]});
        }
    }

    public static void Initialize()
    {
        Hotkeys hotkeys;
        if (File.Exists("./Hotkeys.json"))
            hotkeys = JsonConvert.DeserializeObject<Hotkeys>(File.ReadAllText("./Hotkeys.json"));
        else
            hotkeys = GetDefaultHotkeys();
        SetupHotkeys(hotkeys);
    }
}