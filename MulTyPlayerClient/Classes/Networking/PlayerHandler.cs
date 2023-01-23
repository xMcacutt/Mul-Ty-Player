using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class PlayerHandler
    {
        public static List<Player> Players = new();

        PlayerHandler()
        {
            Players = new();
        }

        public static void AddPlayer(string koalaName, string name, ushort clientId)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Players.Add(new Player(koala, name, clientId));
        }

        public static void RemovePlayer(string name)
        {
            Players.RemoveAll(x => x.Name == name || x.Koala.KoalaName == name);
        }
    }
}
