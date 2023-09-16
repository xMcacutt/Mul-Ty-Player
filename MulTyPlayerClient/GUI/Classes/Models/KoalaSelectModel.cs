﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI.Models
{
    public class KoalaSelectModel
    {
        public KoalaSelectEntryModel Boonie { get; private set; } = new(Koala.Boonie);
        public KoalaSelectEntryModel Dubbo { get; private set; } = new(Koala.Dubbo);
        public KoalaSelectEntryModel Elizabeth { get; private set; } = new(Koala.Elizabeth);
        public KoalaSelectEntryModel Gummy { get; private set; } = new(Koala.Gummy);
        public KoalaSelectEntryModel Katie { get; private set; } = new(Koala.Katie);
        public KoalaSelectEntryModel Kiki { get; private set; } = new(Koala.Kiki);
        public KoalaSelectEntryModel Mim { get; private set; } = new(Koala.Mim);
        public KoalaSelectEntryModel Snugs { get; private set; } = new(Koala.Snugs);

        public event Action<Koala> OnKoalaSelected;
        public event Action OnProceedToLobby;

        public KoalaSelectModel()
        {
            Boonie = new(Koala.Boonie);
            Dubbo = new(Koala.Dubbo);
            Elizabeth = new(Koala.Elizabeth);
            Gummy = new(Koala.Gummy);
            Katie = new(Koala.Katie);
            Kiki = new(Koala.Kiki);
            Mim = new(Koala.Mim);
            Snugs = new(Koala.Snugs);
            Client.HPlayer = new PlayerHandler();
            Client.HKoala = new KoalaHandler();
        }

        public async void KoalaClicked(Koala koala)
        {
            Client.OldKoala = koala;
            bool isHost = !CommandHandler.HostExists();
            PlayerHandler.Players.Add(Client._client.Id, new Player(koala, Client.Name, Client._client.Id, isHost, false));
            ModelController.SFXPlayer.PlaySound(SFX.PlayerConnect);
            OnKoalaSelected?.Invoke(koala);
            PlayerHandler.AnnounceSelection(Koalas.GetInfo[koala].Name, Client.Name, isHost);
            await Task.Delay(2500);
            GetEntry(koala).IsAvailable = false;
            CollectionViewSource.GetDefaultView(ModelController.LoggerInstance.Log).Refresh();
            OnProceedToLobby?.Invoke();
        }

        public bool IsKoalaAvailable(Koala koala)
        {
            return koala switch
            {
                Koala.Boonie => Boonie.IsAvailable,
                Koala.Dubbo => Dubbo.IsAvailable,
                Koala.Elizabeth => Elizabeth.IsAvailable,
                Koala.Gummy => Gummy.IsAvailable,
                Koala.Katie => Katie.IsAvailable,
                Koala.Kiki => Kiki.IsAvailable,
                Koala.Mim => Mim.IsAvailable,
                Koala.Snugs => Snugs.IsAvailable,
                _ => throw new InvalidKoalaException((int)koala),
            };
        }

        public void MakeAllAvailable()
        {
            Boonie.IsAvailable = true;
            Dubbo.IsAvailable = true;
            Elizabeth.IsAvailable = true;
            Gummy.IsAvailable = true;
            Katie.IsAvailable = true;
            Kiki.IsAvailable = true;
            Mim.IsAvailable = true;
            Snugs.IsAvailable = true;
        }

        public void SetAvailability(Koala koala, bool value)
        {
            switch (koala)
            {
                case Koala.Boonie:
                    Boonie.IsAvailable = value; return;
                case Koala.Dubbo:
                    Dubbo.IsAvailable = value; return;
                case Koala.Elizabeth:
                    Elizabeth.IsAvailable = value; return;
                case Koala.Gummy:
                    Gummy.IsAvailable = value; return;
                case Koala.Katie:
                    Katie.IsAvailable = value; return;
                case Koala.Kiki:
                    Kiki.IsAvailable = value; return;
                case Koala.Mim:
                    Mim.IsAvailable = value; return;
                case Koala.Snugs:
                    Snugs.IsAvailable = value; return;
            }

            throw new InvalidKoalaException((int)koala);
        }

        private KoalaSelectEntryModel GetEntry(Koala koala)
        {
            return koala switch
            {
                Koala.Boonie => Boonie,
                Koala.Dubbo => Dubbo,
                Koala.Elizabeth => Elizabeth,
                Koala.Gummy => Gummy,
                Koala.Katie => Katie,
                Koala.Kiki => Kiki,
                Koala.Mim => Mim,
                Koala.Snugs => Snugs,
                _ => throw new InvalidKoalaException((int)koala),
            };
        }
    }

    public class InvalidKoalaException : Exception
    {
        public InvalidKoalaException(int invalidKoalaID) : base(FormatMessage(invalidKoalaID))
        {
        }

        private static string FormatMessage(int invalidKoalaID)
        {
            return $"Tried to retrieve koala information based on invalid koala data.\n" +
                $"Attempted to get koala with ID {invalidKoalaID}. Koala ID's range from 0-7.";
        }
    }
}
