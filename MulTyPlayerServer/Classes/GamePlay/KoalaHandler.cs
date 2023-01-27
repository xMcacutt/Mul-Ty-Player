﻿using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class KoalaHandler
    {
        public static string[] KoalaNames = { "Katie", "Mim", "Elizabeth", "Snugs", "Gummy", "Dubbo", "Kiki", "Boonie" };

        readonly int[] _defaultKoalaPosX = { 250, 0, 0, 0, -2989, -8940, -13646, -572, -3242, -518, -14213, 0, -4246, -5499, -1615, 90, 0, -166, 0, -192, -8845, -82, -82, 10 };
        readonly int[] _defaultKoalaPosY = { 2200, 0, 0, 0, 40, -1653, 138, -695, -809, -2827, 4400, 0, -273, -708, -1488, -789, 0, -100, 0, -630, 1499, 524, 524, -200 };
        readonly int[] _defaultKoalaPosZ = { 6400, 0, 0, 0, 8238, 7162, 22715, -59, 6197, 212, 16627, 0, 1343, -6951, 811, 93, 0, -7041, 0, 3264, 17487, 449, 449, -250 };

        public KoalaHandler()
        {
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void AssignKoala(Message message, ushort fromClientId)
        {
            string koalaName = message.GetString();
            string playerName = message.GetString();
            ushort clientID = message.GetUShort();
            PlayerHandler.AddPlayer(koalaName, playerName, clientID);
            AnnounceKoalaAssigned(koalaName, playerName, clientID, fromClientId, true);
        }

        private static void AnnounceKoalaAssigned(string koalaName, string playerName, ushort clientID, ushort fromToClientId, bool bSendToAll)
        {
            Message announcement = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            announcement.AddString(koalaName);
            announcement.AddString(playerName);
            announcement.AddUShort(clientID);
            if (bSendToAll) Server._Server.SendToAll(announcement, fromToClientId);
            else Server._Server.Send(announcement, fromToClientId);
        }

        public static void SendKoalaAvailability(ushort recipient)
        {
            foreach(Player player in PlayerHandler.Players.Values) AnnounceKoalaAssigned(player.Koala.KoalaName, player.Name, player.ClientID, recipient, false);
        }

        public void ReturnKoala(Player player)
        {
            foreach (Player otherPlayer in PlayerHandler.Players.Values)
            {
                if (otherPlayer.CurrentLevel == player.PreviousLevel && otherPlayer.Name != player.Name)
                {
                    float[] defaultCoords =
                    {
                        _defaultKoalaPosX[player.PreviousLevel],
                        _defaultKoalaPosY[player.PreviousLevel],
                        _defaultKoalaPosZ[player.PreviousLevel],
                        0,
                        0,
                        0
                    };
                    Server.SendCoordinates(player.Koala.KoalaName, player.PreviousLevel, defaultCoords);
                }

            }
        }

    }
}