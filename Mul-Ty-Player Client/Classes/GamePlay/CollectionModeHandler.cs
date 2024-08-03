using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient.Classes.GamePlay;

public class CollectionModeHandler
{
    [MessageHandler((ushort)MessageID.CL_UpdateScore)]
    public static void HandleScoreUpdate(Message message)
    {
        var score = message.GetInt();
        var client = message.GetUShort();
        if (!PlayerHandler.TryGetPlayer(client, out var player))
            return;
        
        player.Score = score;
        if (player.Role != HSRole.Spectator)
            LogScore(player);
    }
    
    private const string LogFilePath = "./Logs/Scores.clm";
    private static readonly object FileLock = new object();
    private static FileStream _fileStream;
    private static void LogScore(Player player)
    {
        lock (FileLock)
        {
            // Ensure file stream is opened
            if (_fileStream is not { CanRead: true } || !_fileStream.CanWrite)
                _fileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            _fileStream.Position = 0; // Reset position for reading
            var lines = new List<string>();
            using (var reader = new StreamReader(_fileStream, Encoding.UTF8, true, 1024, true))
                while (!reader.EndOfStream)
                    lines.Add(reader.ReadLine());

            var playerFound = false;
            for (var i = 0; i < lines.Count; i++)
            {
                if (!lines[i].Equals(player.Name, StringComparison.OrdinalIgnoreCase)) 
                    continue;
                lines[i + 1] = player.Score.ToString();
                playerFound = true;
                break;
            }

            if (!playerFound)
            {
                lines.Add(player.Name);
                lines.Add(player.Score.ToString());
            }

            _fileStream.SetLength(0); // Clear the file before writing
            _fileStream.Position = 0; // Reset position for writing
            using (var writer = new StreamWriter(_fileStream, Encoding.UTF8, 1024, true))
                foreach (var line in lines)
                    writer.WriteLine(line);
        }
    }

    public static void ClearLogs()
    {
        lock (FileLock)
        {
            // Ensure file stream is opened
            if (_fileStream is not { CanWrite: true })
                _fileStream = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            _fileStream.SetLength(0); // Clear the file
            _fileStream.Close();
        }
    }

    public static void Dispose()
    {
        lock (FileLock)
        {
            if (_fileStream == null) 
                return;
            _fileStream.Dispose();
            _fileStream = null;
        }
    }

    [MessageHandler((ushort)MessageID.CL_RuleChange)]
    public static void HandleRuleChange(Message message)
    {
        SFXPlayer.PlaySound(SFX.RuleChange);
        var name = message.GetString();
        var description = message.GetString();
        Client.HGameState.DisplayInGameMessage(name + "\n" + description, 5);
        Logger.Write($"[CLM] Rule Changed: {name}");
        Logger.Write(description);
    }

    [MessageHandler((ushort)MessageID.CL_Start)]
    public static void HandleClmStart(Message message)
    {
        foreach (var entry in PlayerHandler.Players) 
            entry.IsReady = false;
        ModelController.Lobby.IsReady = false;
        ModelController.Lobby.IsReadyButtonEnabled = false;
        Client.HCommand.Commands["tp"].InitExecute(new string[] {"@s"});
        SFXPlayer.PlaySound(SFX.RuleChange);
        Client.HGameState.DisplayInGameMessage("Collection Mode Started\nGo!", 5);
        ResetScores();
    }
    
    [MessageHandler((ushort)MessageID.CL_Stop)]
    public static void HandleClmStop(Message message)
    {
        ModelController.Lobby.IsReadyButtonEnabled = true;
        SFXPlayer.PlaySound(SFX.RuleChange);
        SFXPlayer.PlaySound(SFX.BagCollect);
        Client.HGameState.DisplayInGameMessage("The Collection Mode Round Has Ended");
    }

    public static bool NopalsActive;
    [MessageHandler((ushort)MessageID.CL_Nopals)]
    public static void HandleClmNopals(Message message)
    {
        NopalsActive = message.GetBool();
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x28AB84, new byte[] { 0x1 });
    }

    public static void ResetScores()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_ResetScore);
        Client._client.Send(message);
    }

    public static void RequestAbort()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_Stop);
        Client._client.Send(message);
    }
}