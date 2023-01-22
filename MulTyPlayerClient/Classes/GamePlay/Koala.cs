using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    class Koala
    {
        public string KoalaName;
        public string PlayerName;
        public ushort ClientID;
        public int KoalaID;

        public Koala(string KoalaName, string PlayerName, ushort ClientID, int KoalaID)
        {
            this.KoalaName = KoalaName;
            this.PlayerName = PlayerName;
            this.ClientID = ClientID;
            this.KoalaID = KoalaID;
        }
    }
}
