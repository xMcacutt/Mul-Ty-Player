using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.VisualBasic.Logging;
using System.Timers;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI.Models;
using Octokit;
using Riptide;
using Message = Riptide.Message;

namespace MulTyPlayerClient;

public class MtpCommandAbility : Command
{
    public readonly Stopwatch AbilityStopwatch = new Stopwatch();

    //Used to update ability cooldown display every second
    private Timer AbilityCooldownTimer;

    #region Timer Helper Functions
    public void SetAbilityCooldownTimer()
    {
        AbilityCooldownTimer = new Timer(1000) { AutoReset = true };
        AbilityCooldownTimer.Elapsed += OnAbilityCooldownTimerEvent;

        OnAbilityCooldownTimerChanged?.Invoke((int)Client.HHideSeek.CurrentPerk.AbilityCooldown.TotalSeconds);
        ModelController.Lobby.IsAbilityCooldownVisible = true;

        AbilityCooldownTimer.Start();
    }

    private void OnAbilityCooldownTimerEvent(Object source, ElapsedEventArgs e)
    {
        OnAbilityCooldownTimerChanged?.Invoke((int)GetAbilityCooldownRemaining());
        if (IsAbilityAvailable())
        {
            AbilityCooldownTimer.Stop();
            AbilityCooldownTimer.Dispose();

            ModelController.Lobby.IsAbilityCooldownVisible = false;
        }
    }
    public delegate void AbilityCooldownTimerChangedEventHandler(int value);
    public static event AbilityCooldownTimerChangedEventHandler OnAbilityCooldownTimerChanged;

    private double GetAbilityCooldownRemaining()
    {
        return Math.Round((Client.HHideSeek.CurrentPerk.AbilityCooldown - AbilityStopwatch.Elapsed).TotalSeconds);
    }

    private bool IsAbilityAvailable()
    {
        return AbilityStopwatch.Elapsed >= Client.HHideSeek.CurrentPerk.AbilityCooldown;
    }
    #endregion

    public MtpCommandAbility()
    {
        Name = "ability";
        Aliases = new List<string> { "ab" };
        HostOnly = false;
        SpectatorAllowed = false;
        Usages = new List<string> { "/ability" };
        Description = "Uses your ability if present in Hide & Seek";
        ArgDescriptions = new Dictionary<string, string>
        {
        };
    }
    
    public override void InitExecute(string[] args)
    {
        if (args.Length > 1)
        {
            SuggestHelp();
            return;
        }

        if (SettingsHandler.GameMode != GameMode.HideSeek)
        {
            LogError("Cannot use ability outside of Hide & Seek mode.");
            return;
        }
        if (Client.HGameState.IsOnMainMenuOrLoading)
        {
            LogError("Cannot use ability from the main menu.");
            return;
        }
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            LogError("Could not find self in player list.");
            return;
        }
        if (Client.HHideSeek.Mode != HSMode.SeekTime)
        {
            LogError("You must wait till seek time to use your ability.");
            return;
        }
        
        // Check if enough time has passed since the last taunt
        if (IsAbilityAvailable())
        {
            Client.HHideSeek.CurrentPerk.ApplyAbility();
            AbilityStopwatch.Restart();
            SetAbilityCooldownTimer();
        }
        else
        {
            LogError($"You must wait {GetAbilityCooldownRemaining()} seconds before using your ability.");
        }
    }

    public void StopAbilityCooldownTimer()
    {
        AbilityCooldownTimer.Stop();
        AbilityCooldownTimer.Dispose();
    }
}