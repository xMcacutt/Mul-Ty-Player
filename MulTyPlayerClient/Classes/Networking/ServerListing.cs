using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class ServerListing
    {
        public string IP;
        public string Pass;
        public bool ActiveDefault;
        public ServerListing(string iP, string pass, bool active)
        {
            IP = iP;
            Pass = pass;
            ActiveDefault = active;
        }
    }
}
