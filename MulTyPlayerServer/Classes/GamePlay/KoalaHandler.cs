using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class KoalaHandler
    {
        public static Stack<Koala> availableKoalas;

        readonly int[] _defaultKoalaPosX = { 250, 0, 0, 0, -2989, -8940, -13646, -572, -3242, -518, -14213, 0, -4246, -5499, -1615, 90, 0, -166, 0, -192, -8845, -82, -82, 10 };
        readonly int[] _defaultKoalaPosY = { 2200, 0, 0, 0, 40, -1653, 138, -695, -809, -2827, 4400, 0, -273, -708, -1488, -789, 0, -100, 0, -630, 1499, 524, 524, -200 };
        readonly int[] _defaultKoalaPosZ = { 6400, 0, 0, 0, 8238, 7162, 22715, -59, 6197, 212, 16627, 0, 1343, -6951, 811, 93, 0, -7041, 0, 3264, 17487, 449, 449, -250 };

        public KoalaHandler()
        {
            availableKoalas = new Stack<Koala>();
            Dictionary<string, Koala> koalas = new()
            {
                { "Katie", new Koala(0, "Katie") },
                { "Mim", new Koala(1, "Mim") },
                { "Elizabeth", new Koala(2, "Elizabeth") },
                { "Snugs", new Koala(3, "Snugs") },
                { "Gummy", new Koala(4, "Gummy") },
                { "Dubbo", new Koala(5, "Dubbo") },
                { "Kiki", new Koala(6, "Kiki") },
                { "Boonie", new Koala(7, "Boonie") }
            };
            foreach (string koala in SettingsHandler.KoalaOrder.Reverse())
            {
                availableKoalas.Push(koalas[koala]);
            }
        }

        public void AssignKoala(Player player)
        {
            player.AssignedKoala = availableKoalas.Pop();
        }

        public void ReturnKoala(Player player)
        {
            foreach (Player otherPlayer in Server.PlayerList.Values)
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
                    Server.SendCoordinates(player.AssignedKoala.KoalaId, player.PreviousLevel, defaultCoords, player.Name);
                }

            }
        }

    }
}