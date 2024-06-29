using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HS_DraftViewModel : IViewModel
{
    public HS_PlayerViewModel Team1Player1 { get; set; }
    public HS_PlayerViewModel Team1Player2 { get; set; }
    public HS_PlayerViewModel Team1Player3 { get; set; }
    public HS_PlayerViewModel Team1Player4 { get; set; }
    public HS_PlayerViewModel Team2Player1 { get; set; }
    public HS_PlayerViewModel Team2Player2 { get; set; }
    public HS_PlayerViewModel Team2Player3 { get; set; }
    public HS_PlayerViewModel Team2Player4 { get; set; }
    
    public HS_PickViewModel Ban1 { get; set; }
    public HS_PickViewModel Ban2 { get; set; }
    public HS_PickViewModel Ban3 { get; set; }
    public HS_PickViewModel Ban4 { get; set; }
    public HS_PickViewModel Pick1 { get; set; }
    public HS_PickViewModel Pick2 { get; set; }
    public HS_PickViewModel Pick3 { get; set; }
    public HS_PickViewModel Pick4 { get; set; }
    
    public HS_LevelViewModel LevelZ1 { get; set; }
    public HS_LevelViewModel LevelA1 { get; set; }
    public HS_LevelViewModel LevelA2 { get; set; }
    public HS_LevelViewModel LevelA3 { get; set; }
    public HS_LevelViewModel LevelB1 { get; set; }
    public HS_LevelViewModel LevelB2 { get; set; }
    public HS_LevelViewModel LevelB3 { get; set; }
    public HS_LevelViewModel LevelC1 { get; set; }
    public HS_LevelViewModel LevelC2 { get; set; }
    public HS_LevelViewModel LevelC3 { get; set; }

    public HS_DraftViewModel()
    {
        Team1Player1 = new HS_PlayerViewModel();
        Team1Player2 = new HS_PlayerViewModel();
        Team1Player3 = new HS_PlayerViewModel();
        Team1Player4 = new HS_PlayerViewModel();
        Team2Player1 = new HS_PlayerViewModel();
        Team2Player2 = new HS_PlayerViewModel();
        Team2Player3 = new HS_PlayerViewModel();
        Team2Player4 = new HS_PlayerViewModel();
        Ban1 = new HS_PickViewModel();
        Ban2 = new HS_PickViewModel();
        Ban3 = new HS_PickViewModel();
        Ban4 = new HS_PickViewModel();
        Pick1 = new HS_PickViewModel();
        Pick2 = new HS_PickViewModel();
        Pick3 = new HS_PickViewModel();
        Pick4 = new HS_PickViewModel();
        LevelZ1 = new HS_LevelViewModel();
        LevelA1 = new HS_LevelViewModel();
        LevelA2 = new HS_LevelViewModel();
        LevelA3 = new HS_LevelViewModel();
        LevelB1 = new HS_LevelViewModel();
        LevelB2 = new HS_LevelViewModel();
        LevelB3 = new HS_LevelViewModel();
        LevelC1 = new HS_LevelViewModel();
        LevelC2 = new HS_LevelViewModel();
        LevelC3 = new HS_LevelViewModel();
    }
    
    public void OnEntered()
    {
    }

    public void OnExited()
    {
    }
}