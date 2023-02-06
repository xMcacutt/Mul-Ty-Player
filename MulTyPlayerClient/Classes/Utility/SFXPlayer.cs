using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes.Utility
{
    internal class SFXPlayer
    {
        public static void PlaySound(string path)
        {
            System.Media.SoundPlayer sound = new(path);
            sound.Play();
        }
    }
}
