using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MulTyPlayerClient
{
    internal class SFXPlayer
    {
        public static void PlaySound(string path)
        {
            Task.Run(() =>
            {
                MediaPlayer player = new() { Volume = 0.15 };
                Uri uri = new(path);
                player.Open(uri);
                player.Play();
            });
        }
    }
}
