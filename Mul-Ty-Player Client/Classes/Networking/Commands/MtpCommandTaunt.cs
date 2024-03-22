using System;
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

public class MtpCommandTaunt : Command
{
    public readonly Stopwatch TauntStopwatch = new Stopwatch();
    private readonly TimeSpan tauntCooldown = TimeSpan.FromSeconds(30);
    
    public MtpCommandTaunt()
    {
        Name = "taunt";
        Aliases = new List<string> { "t" };
        HostOnly = false;
        SpectatorAllowed = false;
        Usages = new List<string> { "/taunt" };
        Description = "Taunts the seekers during H&S mode and grants hide time based on your distance to the seeker.";
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

        if (!SettingsHandler.DoHideSeek)
        {
            LogError("Cannot taunt outside of Hide & Seek mode.");
            return;
        }
        if (Client.HGameState.IsOnMainMenuOrLoading)
        {
            LogError("Cannot taunt from the main menu.");
            return;
        }
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            LogError("Could not find self in player list.");
            return;
        }
        if (Client.HHideSeek.Mode != HSMode.SeekTime)
        {
            LogError("You must wait till seek time to taunt.");
            return;
        }
        if (self.Role == HSRole.Seeker)
        {
            LogError("Cannot taunt as seeker.");
            return;
        }
        
        // Check if enough time has passed since the last taunt
        if (TauntStopwatch.Elapsed >= tauntCooldown)
        {
            RunTaunt();
            TauntStopwatch.Restart(); // Restart the stopwatch after calling RunTaunt
        }
        else
        {
            var remainingTime = tauntCooldown - TauntStopwatch.Elapsed;
            LogError($"You must wait {Math.Round(remainingTime.TotalSeconds)} seconds before taunting.");
        }
    }

    private void RunTaunt()
    {
        var overallTimeBonus = 0;
        var seekerCount = 0;
        foreach (var player in PlayerHandler.Players.Where(x => x.Role == HSRole.Seeker))
        {
            var ownPosition = Client.HHero.GetCurrentPosRot();
            var koalaId = Koalas.GetInfo[(Koala)player.Koala].Id;
            var transform = PlayerReplication.PlayerTransforms[koalaId];
            var seekerPosition = transform.Position.AsFloats();
            var distance = Vector3.Distance(new Vector3(ownPosition), new Vector3(seekerPosition));
            var timeBonus = distance switch
            {
                < 1000 => 10,
                < 2000 => 5,
                < 3000 => 2,
                _ => 0
            };
            Client.HHideSeek.Time += timeBonus;
            if (timeBonus > 0)
            {
                var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_Taunt);
                message.AddUShort(player.Id);
                message.AddFloat(distance);
                Client._client.Send(message);
                overallTimeBonus += timeBonus;
                seekerCount++;
            }
        }
        if (seekerCount == 0)
        {
            Logger.Write($"[HIDE AND SEEK] No seekers were nearby to hear your screams. Womp womp.");
            return;
        }
        Logger.Write($"[HIDE AND SEEK] {seekerCount} seekers heard you. Bonus time of {overallTimeBonus} awarded for taunt.");
    }

    [MessageHandler((ushort)MessageID.HS_Taunt)]
    private static void HearTaunt(Message message)
    {
        var distance = message.GetFloat();
        float max = 3000f;
        float min = 1000f;
        float volume = distance >= max ? 0.0f :
                distance <= min ? 0.5f :
                    0.5f - (distance - min) / (max - min);
        Logger.Write($"[HIDE AND SEEK] There is a player {distance} units away.");
        SFXPlayer.PlaySound(SFX.Taunt, volume);
    }
}