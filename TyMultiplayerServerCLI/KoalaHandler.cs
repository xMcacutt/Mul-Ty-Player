using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerServerCLI
{
    internal class KoalaHandler
    {
        public static Stack<Koala> availableKoalas;

        public int[] defaultKoalaPosX = { 250, 0, 0, 0, -2989, -8940, -13646, -572, -3242, -518, -14213, 0, -4246, -5499, -1615, 90, 0, -166, 0, -192, -8845, -82, -82, 10 };
        public int[] defaultKoalaPosY = { 2200, 0, 0, 0, 40, -1653, 138, -695, -809, -2827, 4400, 0, -273, -708, -1488, -789, 0, -100, 0, -630, 1499, 524, 524, -200 };
        public int[] defaultKoalaPosZ = { 6400, 0, 0, 0, 8238, 7162, 22715, -59, 6197, 212, 16627, 0, 1343, -6951, 811, 93, 0, -7041, 0, 3264, 17487, 449, 449, -250 };


        public KoalaHandler()
        {
            availableKoalas = new Stack<Koala>();

            availableKoalas.Push(new Koala(0, "Boonie"));
            availableKoalas.Push(new Koala(1, "Mim"));
            availableKoalas.Push(new Koala(2, "Kiki"));
            availableKoalas.Push(new Koala(3, "Katie"));
            availableKoalas.Push(new Koala(4, "Snugs"));
            availableKoalas.Push(new Koala(5, "Dubbo"));
            availableKoalas.Push(new Koala(6, "Gummy"));
            availableKoalas.Push(new Koala(7, "Elizabeth"));
        }

        public void AssignKoala(Player player)
        {
            player.assignedKoala = availableKoalas.Pop();
            player.assignedKoala.assignedPlayer = player;
        }

        public void ReturnKoala(Player player)
        {
            foreach (Player otherPlayer in Program.playerList.Values)
            {
                if (otherPlayer.currentLevel == player.previousLevel && otherPlayer.name != player.name)
                {
                    float[] defaultCoords =
                    {
                        defaultKoalaPosX[player.previousLevel],
                        defaultKoalaPosY[player.previousLevel],
                        defaultKoalaPosZ[player.previousLevel],
                        0
                    };
                    Program.SendCoordinates(player.assignedKoala.koalaID, player.previousLevel, defaultCoords, player.name);
                    Program.SendMessageToClients($"Removed {player.name} from {otherPlayer.name}'s level", true);
                }

            }
        }

    }
}