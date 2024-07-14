using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer.Classes.Networking;

public class HSD_DraftsHandler
{
    public List<ushort> Team1;
    public List<ushort> Team2;
    public List<Pick> Picks;

    public HSD_DraftsHandler()
    {
        Team1 = new List<ushort>();
        Team2 = new List<ushort>();
        Picks = new List<Pick>();
    }

    public void AddPlayerToTeam(ushort clientId, HSD_Team team)
    {
        switch (team)
        {
            case HSD_Team.Team1 when Team1.Count < 4:
                Team1.Add(clientId);
                break;
            case HSD_Team.Team2 when Team2.Count < 4:
                Team2.Add(clientId);
                break;
            default:
                Console.WriteLine("Team does not exist.");
                break;
        }
    }

    public void RemovePlayerFromTeam(ushort clientId, HSD_Team team)
    {
        switch (team)
        {
            case HSD_Team.Team1 when Team1.Contains(clientId):
                Team1.Remove(clientId);
                break;
            case HSD_Team.Team2 when Team2.Contains(clientId):
                Team2.Remove(clientId);
                break;
            default:
                Console.WriteLine("Team does not exist.");
                break;
        }
    }

    public void AnnouncePlayerJoined(ushort clientId, HSD_Team team)
    {        
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_PlayerJoined);
        message.AddUShort(clientId);
        message.AddInt((int)team);
        Server._Server.SendToAll(message);
    }

    public void AnnouncePlayerLeft(ushort clientId, HSD_Team team)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_PlayerLeft);
        message.AddUShort(clientId);
        message.AddInt((int)team);
        Server._Server.SendToAll(message, clientId);
    }
    
    public void SendTeamData(ushort toClientId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_TeamData);
        message.AddUShorts(Team1.ToArray());
        message.AddUShorts(Team2.ToArray());
        message.AddInt(Picks.Count);
        foreach (var p in Picks)
        {
            message.AddInt(p.Level);
            message.AddInt((int)p.Team);
            message.AddInt((int)p.PickType);
        }
        message.AddInt((int)currentTeam);
        message.AddUShort(Program.HDrafts.Team1.Count > 0 ? Program.HDrafts.Team1[currentTeam1PlayerIndex] : (ushort)0);
        message.AddUShort(Program.HDrafts.Team2.Count > 0 ? Program.HDrafts.Team2[currentTeam2PlayerIndex] : (ushort)0);
        Server._Server.Send(message, toClientId);
    }

    public bool TryRemovePlayer(ushort clientId)
    {
        HSD_Team? team = null;
        if (Team1.Contains(clientId))
            team = HSD_Team.Team1;
        if (Team2.Contains(clientId))
            team = HSD_Team.Team2;
        if (team == null)
            return false;
        RemovePlayerFromTeam(clientId, (HSD_Team)team);
        AnnouncePlayerLeft(clientId, (HSD_Team)team);
        return true;
    }

    [MessageHandler((ushort)MessageID.HSD_PlayerJoined)]
    public static void HandlePlayerJoined(ushort fromClientId, Message message)
    {
        var team = (HSD_Team)message.GetInt();
        Program.HDrafts.AddPlayerToTeam(fromClientId, team);
        Program.HDrafts.AnnouncePlayerJoined(fromClientId, team);
    }
    
    [MessageHandler((ushort)MessageID.HSD_PlayerLeft)]
    public static void HandlePlayerLeft(ushort fromClientId, Message message)
    {
        Program.HDrafts.TryRemovePlayer(fromClientId);
    }

    [MessageHandler((ushort)MessageID.HSD_PickRequest)]
    public static void HandlePickRequest(ushort fromClientId, Message message)
    {
        var levelId = message.GetInt();
        HSD_Team team;
        if (Program.HDrafts.Team1.Contains(fromClientId))
            team = HSD_Team.Team1;
        else if (Program.HDrafts.Team2.Contains(fromClientId))
            team = HSD_Team.Team2;
        else return;
        Program.HDrafts.Picks.Add(new Pick(levelId, team, HSD_Pick.Pick));
        var pick = Program.HDrafts.Picks.Count < 5 ? HSD_Pick.Ban : HSD_Pick.Pick;
        Message response = Message.Create(MessageSendMode.Reliable, MessageID.HSD_PickRequest);
        response.AddInt(levelId);
        response.AddInt((int)team);
        response.AddInt((int)pick);
        GetNextPlayer();
        response.AddInt((int)currentTeam);
        response.AddUShort(Program.HDrafts.Team1.Count > 0 ? Program.HDrafts.Team1[currentTeam1PlayerIndex] : (ushort)0);
        response.AddUShort(Program.HDrafts.Team2.Count > 0 ? Program.HDrafts.Team2[currentTeam2PlayerIndex] : (ushort)0);
        Server._Server.SendToAll(response);

        if (Program.HDrafts.Picks.Count == 8)
        {
            var thread = new Thread(new ThreadStart(ResetDraftsAsync));
            thread.Start();
        }
    }

    private static async void ResetDraftsAsync()
    {
        await Task.Delay(5000); // Delay for 5 seconds
        Program.HDrafts.Picks = new List<Pick>();
        Program.HDrafts.Team1 = new List<ushort>();
        Program.HDrafts.Team2 = new List<ushort>();
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_Reset);
        Server._Server.SendToAll(message);
    }

    private static void GetNextPlayer()
    {
        switch (Program.HDrafts.Picks.Count)
        {
            case 1 or 3 or 5 or 7:
                if (currentTeam == HSD_Team.Team1)
                {
                    currentTeam1PlayerIndex = (currentTeam1PlayerIndex + 1) % Program.HDrafts.Team1.Count;
                    currentTeam = HSD_Team.Team2;
                }
                else if (currentTeam == HSD_Team.Team2)
                {
                    currentTeam2PlayerIndex = (currentTeam2PlayerIndex + 1) % Program.HDrafts.Team2.Count;
                    currentTeam = HSD_Team.Team1;
                }
                break;
            case 2 or 4 or 6 or 8:
                if (currentTeam == HSD_Team.Team1)
                    currentTeam1PlayerIndex = (currentTeam1PlayerIndex + 1) % Program.HDrafts.Team1.Count;
                else if (currentTeam == HSD_Team.Team2)
                    currentTeam2PlayerIndex = (currentTeam2PlayerIndex + 1) % Program.HDrafts.Team2.Count;
                break;
        }
    }

    private static int currentTeam1PlayerIndex = 0;
    private static int currentTeam2PlayerIndex = 0;
    private static HSD_Team currentTeam;
    [MessageHandler((ushort)MessageID.HSD_Start)]
    public static void HandleStartRequest(ushort fromClientId, Message message)
    {
        Program.HDrafts.Picks.Clear();
        var rand = new Random();
        currentTeam = (HSD_Team)rand.Next(1, 2);
        if (currentTeam == HSD_Team.Team1)
            currentTeam1PlayerIndex = rand.Next(0, Program.HDrafts.Team1.Count - 1);
        if (currentTeam == HSD_Team.Team2)
            currentTeam2PlayerIndex = rand.Next(0, Program.HDrafts.Team2.Count - 1);
        Message startMessage = Message.Create(MessageSendMode.Reliable, MessageID.HSD_Start);
        startMessage.AddInt((int)currentTeam);
        startMessage.AddUShort(Program.HDrafts.Team1.Count > 0 ? Program.HDrafts.Team1[currentTeam1PlayerIndex] : (ushort)0);
        startMessage.AddUShort(Program.HDrafts.Team2.Count > 0 ? Program.HDrafts.Team2[currentTeam2PlayerIndex] : (ushort)0);
        Server._Server.SendToAll(startMessage);
    }
}

public class Pick
{
    public int Level;
    public HSD_Team Team;
    public HSD_Pick PickType;

    public Pick(int level, HSD_Team team, HSD_Pick pick)
    {
        Level = level;
        Team = team;
        PickType = pick;
    }
}

public enum HSD_Pick
{
    Pick,
    Ban
}

public enum HSD_Team
{
    NoTeam,
    Team1,
    Team2
}

public enum HSD_State
{
    Joining,
    Picks,
}