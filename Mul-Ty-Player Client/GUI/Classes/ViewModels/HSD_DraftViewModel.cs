using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_DraftViewModel : IViewModel
{
    private static HSD_DraftModel DraftsModel => ModelController.HSD_Draft;
    public HSD_LevelViewModel LevelZ1 { get; set; }
    public HSD_LevelViewModel LevelA1 { get; set; }
    public HSD_LevelViewModel LevelA2 { get; set; }
    public HSD_LevelViewModel LevelA3 { get; set; }
    public HSD_LevelViewModel LevelB1 { get; set; }
    public HSD_LevelViewModel LevelB2 { get; set; }
    public HSD_LevelViewModel LevelB3 { get; set; }
    public HSD_LevelViewModel LevelC1 { get; set; }
    public HSD_LevelViewModel LevelC2 { get; set; }
    public HSD_LevelViewModel LevelC3 { get; set; }
    public bool BlockLevelSelect { get; set; }
    public bool IsSwapTeam2Enabled { get; set; }
    public bool IsSwapTeam1Enabled { get; set; }
    public bool ReadyToStart { get; set; }


    private HSD_PlayerViewModel CreatePlayerViewModel(ObservableCollection<ushort> team, int index, HSD_Team teamEnum)
    {
        if (team.Count > index && PlayerHandler.TryGetPlayer(team[index], out var player))
        {
            return new HSD_PlayerViewModel(new HSD_PlayerModel(player), teamEnum);
        }
        return new HSD_PlayerViewModel(new HSD_PlayerModel(null), teamEnum);
    }
    
    public HSD_DraftViewModel()
    {
        DraftsModel.OnBlockLevelSelectChanged += Model_BlockLevelSelectChanged;
        DraftsModel.OnSwapTeam1EnabledChanged += Model_SwapTeam1EnabledChanged;
        DraftsModel.OnSwapTeam2EnabledChanged += Model_SwapTeam2EnabledChanged;
        DraftsModel.OnReadyToStartChanged += Model_ReadyToStartChanged;
        LevelZ1 = new HSD_LevelViewModel(DraftsModel.LevelZ1);
        LevelA1 = new HSD_LevelViewModel(DraftsModel.LevelA1);
        LevelA2 = new HSD_LevelViewModel(DraftsModel.LevelA2);
        LevelA3 = new HSD_LevelViewModel(DraftsModel.LevelA3);
        LevelB1 = new HSD_LevelViewModel(DraftsModel.LevelB1);
        LevelB2 = new HSD_LevelViewModel(DraftsModel.LevelB2);
        LevelB3 = new HSD_LevelViewModel(DraftsModel.LevelB3);
        LevelC1 = new HSD_LevelViewModel(DraftsModel.LevelC1);
        LevelC2 = new HSD_LevelViewModel(DraftsModel.LevelC2);
        LevelC3 = new HSD_LevelViewModel(DraftsModel.LevelC3);
    }

    private void Model_ReadyToStartChanged(bool value)
    {
        ReadyToStart = value;
    }

    private void Model_BlockLevelSelectChanged(bool value)
    {
        BlockLevelSelect = value;
    }

    private void Model_SwapTeam1EnabledChanged(bool value)
    {
        IsSwapTeam1Enabled = value;
    }
    
    private void Model_SwapTeam2EnabledChanged(bool value)
    {
        IsSwapTeam2Enabled = value;
    }
    
    public void OnEntered()
    {
    }

    public void OnExited()
    {
    }
}