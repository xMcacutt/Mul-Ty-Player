using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MulTyPlayerClient.Classes.Utility
{
    internal class SFXPlayer
    {
        static MediaPlayer player = new();

        public static void PlaySound(string path)
        {
            Uri uri = new Uri(path);
            player.Open(uri);
            player.Volume = 0.1;
            player.Play();
        }
    }
}
