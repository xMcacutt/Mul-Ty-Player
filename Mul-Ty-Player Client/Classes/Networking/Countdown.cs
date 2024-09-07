using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient.Classes;

public class Countdown
{
    private static CancellationTokenSource abortTokenSource = new();

    public static bool InProgress { get; private set; }

    public static event Action OnCountdownBegan;
    public static event Action OnCountdownAborted;
    public static event Action OnCountdownFinished;

    [MessageHandler((ushort)MessageID.Countdown)]
    public static void HandleCountdown(Message message)
    {
        var param = message.GetString();
        if (string.Equals(param, "start", StringComparison.CurrentCultureIgnoreCase) && !InProgress)
            StartCountdown();
        else if (!InProgress)
            Logger.Write("[WARN] A countdown abort attempt was made with no countdown running.");    
        else if (string.Equals(param, "abort", StringComparison.CurrentCultureIgnoreCase))
            Abort();
    }

    private static async void StartCountdown()
    {
        InProgress = true;
        abortTokenSource = new CancellationTokenSource();
        var abortToken = abortTokenSource.Token;

        foreach (var entry in PlayerHandler.Players) entry.IsReady = false;
        ModelController.Lobby.IsReady = false;
        ModelController.Lobby.OnLogout += Abort;

        var countdown = Task.Run(() =>
        {
            abortToken.ThrowIfCancellationRequested();
            OnCountdownBegan?.Invoke();
            Client.HSync = new SyncHandler();
            SFXPlayer.StopAll();
            Client.HHero.SetDefaults();
            SFXPlayer.PlaySound(SFX.Race10);
            Client.HGameState.ForceBackToMainMenu();
            for (var i = 10; i > 0; i--)
            {
                if (i < 8 && CheckAnyPlayerOnMainMenu()) Abort();

                if (abortToken.IsCancellationRequested) abortToken.ThrowIfCancellationRequested();

                Logger.Write(i.ToString());
                if (i == 5)
                    Client.HGameState.ForceEnterNewGameScreen();
                if (i == 3)
                {
                    Client.HGameState.ForcePrepareNewGame(SettingsHandler.ClientSettings.DefaultSaveSlot);
                    Client.HSync = new SyncHandler();
                    if (Client._client.Id == PlayerHandler.Players[0].Id)
                    {
                        var message = Message.Create(MessageSendMode.Reliable, MessageID.CountdownFinishing);
                        Client._client.Send(message);
                    }
                    SFXPlayer.StopAll();
                    SFXPlayer.PlaySound(SFX.Race321);
                }

                Task.Delay(1000).Wait();
            }

            Client.HGameState.ForceNewGame();
            Logger.Write("Go!");
        }, abortToken);

        try
        {
            await countdown;
            OnCountdownFinished?.Invoke();
        }
        catch (OperationCanceledException)
        {
            Logger.Write("Countdown aborted.");
        }
        finally
        {
            abortTokenSource.Dispose();
            ModelController.Lobby.IsReady = true;
            ModelController.Lobby.OnLogout -= Abort;
            InProgress = false;
        }
    }

    private static void Abort()
    {
        SFXPlayer.StopSound(SFX.Race10);
        SFXPlayer.StopSound(SFX.Race321);
        SFXPlayer.PlaySound(SFX.RaceAbort);
        abortTokenSource.Cancel();
        OnCountdownAborted?.Invoke();
    }

    private static bool CheckAnyPlayerOnMainMenu()
    {
        return !Client.HGameState.IsAtMainMenu() || PlayerHandler.Players.Any(p => p.Level != "M/L");
    }
}