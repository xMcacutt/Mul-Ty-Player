﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Networking;
using Octokit;
using Riptide;
using Message = Riptide.Message;

namespace MulTyPlayerClient;

public class MtpCommandAbility : Command
{
    public readonly Stopwatch AbilityStopwatch = new Stopwatch();
    
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
        if (AbilityStopwatch.Elapsed >= Client.HHideSeek.CurrentPerk.AbilityCooldown)
        {
            Client.HHideSeek.CurrentPerk.ApplyAbility();
            AbilityStopwatch.Restart();
        }
        else
        {
            var remainingTime = Client.HHideSeek.CurrentPerk.AbilityCooldown - AbilityStopwatch.Elapsed;
            LogError($"You must wait {Math.Round(remainingTime.TotalSeconds)} seconds before using your ability.");
        }
    }
}