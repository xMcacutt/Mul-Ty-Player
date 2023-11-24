using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MulTyPlayerClient
{
    public enum SFX
    {
        PlayerConnect,
        PlayerDisconnect,
        MenuClick1,
        MenuClick2,
        MenuAccept,
        MenuCancel,
        Race321,
        Race10,
        RaceAbort
    }

    public static class SFXPlayer
    {
        public static void Init()
        {
            mediaPlayers = new();

            foreach (var kvPair in sfxResources)
            {
                AddSound(kvPair.Key, kvPair.Value, false);
            }
        }

        readonly static Dictionary<SFX, Uri> sfxResources = new Dictionary<SFX, Uri>()
        {
            {SFX.PlayerConnect,      new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/PlayerConnect.wav") },
            {SFX.PlayerDisconnect,   new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/PlayerDisconnect.wav") },
            {SFX.MenuClick1,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuClick1.wav") },
            {SFX.MenuClick2,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuClick2.wav") },
            {SFX.MenuAccept,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuAccept.wav") },
            {SFX.MenuCancel,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuCancel.wav") },
            {SFX.Race321,            new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Race321.wav") },
            {SFX.Race10,             new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Race10.wav") },
            {SFX.RaceAbort,          new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/RaceAbort.wav") }
        };

        static Dictionary<SFX, MediaPlayer> mediaPlayers;

        public static void PlaySound(SFX sfxName)
        {
            if (mediaPlayers.TryGetValue(sfxName, out MediaPlayer player))
            {
                player.Dispatcher.Invoke(() =>
                {
                    player.Stop();
                    player.Play();
                });
            }
        }

        public static void StopAll()
        {
            foreach (var player in mediaPlayers.Values)
            {
                player.Dispatcher.Invoke(player.Stop);
            }
        }

        public static void StopSound(SFX sfxName)
        {
            if (mediaPlayers.TryGetValue(sfxName, out MediaPlayer player))
            {
                player.Dispatcher.Invoke(player.Stop);
            }
        }

        private static void AddSound(SFX sfxName, Uri uri, bool overrideOldSFX=false)
        {
            if (overrideOldSFX)
            {
                //If overriding this sfxType, remove the old sound
                               
            }
            else if (mediaPlayers.ContainsKey(sfxName))
            {
                //If not overriding and mediaplayer already contains the sfx, exit
                return;
            }
            MediaPlayer mp = new();
            mp.Open(uri);
            mp.Volume = 0.15f;
            mediaPlayers.Add(sfxName, mp);
        }

        //Returns true if sound successfully removed
        //False if not
        private static bool RemoveSound(SFX sfxName)
        {
            if (mediaPlayers.TryGetValue(sfxName, out MediaPlayer player))
            {
                player.Stop();
                player.Close();
                return mediaPlayers.Remove(sfxName);
            }
            return false;
        }
    }
}
