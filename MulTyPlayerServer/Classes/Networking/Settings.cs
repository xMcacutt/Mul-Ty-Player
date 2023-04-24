﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer 
{ 
    internal class Settings
    {
        public string Password { get; set; }
        public ushort Port { get; set; }
        public bool DoSyncTEs { get; set; }
        public bool DoSyncCogs { get; set; }
        public bool DoSyncBilbies { get; set; }
        public bool DoSyncRangs { get; set; }
        public bool DoSyncOpals { get; set; }
        public bool DoSyncPortals { get; set; }
        public bool DoSyncCliffs { get; set; }
        public bool DoSyncCrate { get; set; }
    }
}