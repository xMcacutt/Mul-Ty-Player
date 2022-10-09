using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerServerCLI
{
    internal class Koala
    { 
        public Player assignedPlayer;
        public float[] coordinates;
        public int koalaID;
        public string name;

        public Koala(int koalaID, string name)
        {
            this.koalaID = koalaID;
            this.name = name;
            coordinates = new float[4];
        }
    }
}
