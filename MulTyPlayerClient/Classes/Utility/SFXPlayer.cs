using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

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
    }

    public class SFXPlayer
    {
        readonly static Dictionary<SFX, Uri> sounds = new Dictionary<SFX, Uri>()
        {
            {SFX.PlayerConnect,      new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/PlayerConnect.wav") },
            {SFX.PlayerDisconnect,   new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/PlayerDisconnect.wav") },
            {SFX.MenuClick1,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuClick1.wav") },
            {SFX.MenuClick2,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuClick2.wav") },
            {SFX.MenuAccept,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuAccept.wav") },
            {SFX.MenuCancel,         new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuCancel.wav") },
            {SFX.Race321,            new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Race321.wav") },
            {SFX.Race10,             new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Race10.wav") },
        };

        MediaPlayer player = new() { Volume = 0.15 };

        public void PlaySound(SFX sfxName)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                player.MediaEnded += (o, e) => player.Close();
                player.Open(sounds[sfxName]);
                player.Play();
            });
        }


        public void Stop()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                player.Stop();
            });            
        }
    }
}
