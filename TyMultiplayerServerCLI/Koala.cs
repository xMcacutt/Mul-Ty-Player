using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerServerCLI
{
    internal class Koala
    { 
        public float[] Coordinates;
        public int KoalaId;
        public string Name;

        public Koala(int koalaID, string name)
        {
            KoalaId = koalaID;
            Name = name;
            Coordinates = new float[6];
        }
    }
}
