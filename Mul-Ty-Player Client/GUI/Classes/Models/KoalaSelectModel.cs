﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MulTyPlayerClient.Classes.Utility;

namespace MulTyPlayerClient.GUI.Models;

public class KoalaSelectModel
{
    public KoalaSelectModel()
    {
        Boonie = new KoalaSelectEntryModel(Koala.Boonie);
        Dubbo = new KoalaSelectEntryModel(Koala.Dubbo);
        Elizabeth = new KoalaSelectEntryModel(Koala.Elizabeth);
        Gummy = new KoalaSelectEntryModel(Koala.Gummy);
        Katie = new KoalaSelectEntryModel(Koala.Katie);
        Kiki = new KoalaSelectEntryModel(Koala.Kiki);
        Mim = new KoalaSelectEntryModel(Koala.Mim);
        Snugs = new KoalaSelectEntryModel(Koala.Snugs);
        MakeAllAvailable();
    }

    public KoalaSelectEntryModel Boonie { get; } = new(Koala.Boonie);
    public KoalaSelectEntryModel Dubbo { get; } = new(Koala.Dubbo);
    public KoalaSelectEntryModel Elizabeth { get; } = new(Koala.Elizabeth);
    public KoalaSelectEntryModel Gummy { get; } = new(Koala.Gummy);
    public KoalaSelectEntryModel Katie { get; } = new(Koala.Katie);
    public KoalaSelectEntryModel Kiki { get; } = new(Koala.Kiki);
    public KoalaSelectEntryModel Mim { get; } = new(Koala.Mim);
    public KoalaSelectEntryModel Snugs { get; } = new(Koala.Snugs);

    public event Action<Koala> OnKoalaSelected;
    public event Action OnProceedToLobby;

    public async void KoalaClicked(Koala koala, bool proceedToLobby)
    {
        Client.OldKoala = koala;
        PlayerHandler.Players.Add(new Player(koala, Client.Name, Client._client.Id, false, false, HSRole.Hider, 0, Client.VIP));
        PlayerHandler.AnnounceSelection(koala, Client.Name, false, false, HSRole.Hider, 0, Client.VIP);
        OnKoalaSelected?.Invoke(koala);
        var sound = Client.VIP == VIP.None ? SFX.PlayerConnect : VIPHandler.GetSound(Client.VIP);
        SFXPlayer.PlaySound(sound);
        
        if (proceedToLobby)
            await Task.Delay(2125);
        
        GetEntry(koala).SetAvailability(false);
        
        if (proceedToLobby)
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
            _ => throw new InvalidKoalaException((int)koala)
        };
    }

    public void MakeAllAvailable()
    {
        BlockKoalaSelect = false;
        Boonie.SetAvailability(true);
        Dubbo.SetAvailability(true);
        Elizabeth.SetAvailability(true);
        Gummy.SetAvailability(true);
        Katie.SetAvailability(true);
        Kiki.SetAvailability(true);
        Mim.SetAvailability(true);
        Snugs.SetAvailability(true);
    }

    public void SetAvailability(Koala koala, bool value)
    {
        Debug.WriteLine("Koala: " + koala);
        Debug.WriteLine("Available: " + value);
        switch (koala)
        {
            case Koala.Boonie:
                Boonie.SetAvailability(value);
                return;
            case Koala.Dubbo:
                Dubbo.SetAvailability(value);
                return;
            case Koala.Elizabeth:
                Elizabeth.SetAvailability(value);
                return;
            case Koala.Gummy:
                Gummy.SetAvailability(value);
                return;
            case Koala.Katie:
                Katie.SetAvailability(value);
                return;
            case Koala.Kiki:
                Kiki.SetAvailability(value);
                return;
            case Koala.Mim:
                Mim.SetAvailability(value);
                return;
            case Koala.Snugs:
                Snugs.SetAvailability(value);
                return;
        }

        throw new InvalidKoalaException((int)koala);
    }

    
    public event Action<bool> OnBlockKoalaSelectChanged;
    private bool blockKoalaSelect;
    public bool BlockKoalaSelect
    {
        get => blockKoalaSelect;
        set
        {
            blockKoalaSelect = value;
            OnBlockKoalaSelectChanged?.Invoke(blockKoalaSelect);
        }
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
            _ => throw new InvalidKoalaException((int)koala)
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