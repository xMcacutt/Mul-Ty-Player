using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MulTyPlayer;
using MulTyPlayerClient.Classes.GamePlay;
using MulTyPlayerClient.GUI.Controls;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using Riptide;

namespace MulTyPlayerClient.GUI.Views;

/// <summary>
///     Interaction logic for ClientGUI.xaml
/// </summary>
public partial class Lobby : UserControl
{
    private Player clickedPlayer;

    public Lobby()
    {
        InitializeComponent();
        if (!SettingsHandler.Settings.AutoJoinVoice) return;
        MTPAudioToggle.IsChecked = true;
        //VoiceHandler.JoinVoice();
    }

    private void TextboxInput_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Return:
                LobbyModel.ProcessInput(TextBoxInput.Text);
                TextBoxInput.Text = "";
                break;
            case Key.Up or Key.Down when !CommandCompletePopup.IsOpen:
                var recallText = LobbyModel.ProcessRecall(e.Key == Key.Up);
                if (e.Key == Key.Up && string.IsNullOrWhiteSpace(recallText))
                    break;
                TextBoxInput.Text = recallText;
                TextBoxInput.CaretIndex = TextBoxInput.Text.Length;
                break;
            case Key.Escape when CommandCompletePopup.IsOpen:
                CommandCompletePopup.IsOpen = false;
                break;
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        App.SettingsWindow = new SettingsMenu();
        Application.Current.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(ModelController.Settings.UpdateHost));
        ModelController.Settings.SetPropertiesFromSettings();
        App.SettingsWindow.ShowDialog();
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta != 0)
            e = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta * -1);
        base.OnPreviewMouseWheel(e);
    }

    private void ReadyButton_Click(object sender, RoutedEventArgs e)
    {
        Client.HPlayer.SetReady();
    }

    private void RequestSyncButton_Click(object sender, RoutedEventArgs e)
    {
        Client.HSync.RequestSync();
        Logger.Write("Sync request sent to server.");
    }

    private void PlayerListItem_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var row = FindAncestor<DataGridRow>((DependencyObject)e.OriginalSource);
        if (row is null) return;
        clickedPlayer = row.Item as Player;
    }

    private static T FindAncestor<T>(DependencyObject current)
        where T : DependencyObject
    {
        do
        {
            if (current is T ancestor)
                return ancestor;

            current = VisualTreeHelper.GetParent(current);
        } while (current != null);

        return null;
    }

    private void GiveHost_OnClick(object sender, RoutedEventArgs e)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.GiftHost);
        message.AddUShort(clickedPlayer.Id);
        ModelController.Lobby.IsHost = false;
        Client._client.Send(message);
    }

    private void KickPlayer_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        Client.HCommand.Commands["kick"].InitExecute(new string[] { clickedPlayer.Id.ToString() });
    }

    private void ForceMenu_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HPlayer.ForceToMenu(clickedPlayer.Id);
    }

    private void LevelLockToggle_Click(object sender, RoutedEventArgs e)
    {
        var value = SettingsHandler.DoLevelLock ? "False" : "True";
        Client.HCommand.Commands["levellock"].InitExecute(new string[] { value });
    }
    
    private void NoMode_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["gamemode"].InitExecute(new [] { "Normal" });
    }
    
    private void HideSeekMode_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["gamemode"].InitExecute(new [] { "HideSeek" });
    }
    
    private void ChaosMode_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["gamemode"].InitExecute(new [] { "Chaos" });
    }
    
    private void CollectionMode_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["gamemode"].InitExecute(new [] { "Collection" });
    }

    private void ClearPassword_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["password"].InitExecute(new string[] {"default"});

    }

    private void ResetSync_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["resetsync"].InitExecute(Array.Empty<string>());
    }

    private void DataGrid_OnContextMenuOpening(object sender, RoutedEventArgs e)
    {
        if (DataContext is not LobbyViewModel viewModel || clickedPlayer == null || clickedPlayer.Id == Client._client.Id || !viewModel.IsHostMenuButtonEnabled)
        {
            e.Handled = true;
            HostDataGridContextMenu.IsOpen = false;
            return;
        }
        HostDataGridContextMenu.IsOpen = true;
    }

    private void HostMenuButton_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        LevelLockToggle.IsChecked = SettingsHandler.DoLevelLock;
    }
    
    #region Voice

    private void MTPAudioToggle_Click(object sender, RoutedEventArgs e)
    {
        MTPAudioToggle.IsChecked = !MTPAudioToggle.IsChecked;
        if (MTPAudioToggle.IsChecked)
        {
            // VoiceHandler.JoinVoice();
            return;
        } 
        //  VoiceHandler.LeaveVoice();
    }

    private void Proximity_Click(object sender, RoutedEventArgs e)
    {
        // VoiceHandler.DoProximityCheck = ProximityToggle.IsChecked;
    }
    #endregion
    

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element) return;
        element.ContextMenu.PlacementTarget = element;
        element.ContextMenu.IsOpen = true;
    }

    private void ChangeRoleToggle_Click(object sender, RoutedEventArgs e)
    {
        if (ModelController.Login.JoinAsSpectator)
            Client.HHideSeek.Role = HSRole.Spectator;
        Client.HHideSeek.Role = Client.HHideSeek.Role == HSRole.Hider ? HSRole.Seeker : HSRole.Hider;
    }

    private void TimerToggle_Click(object sender, RoutedEventArgs e)
    {
        ModelController.Lobby.IsTimerVisible = !ModelController.Lobby.IsTimerVisible;
    }

    private void ResetTimer_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HHideSeek.Time = 0;
    }

    private void ShuffleSeed_Click(object sender, RoutedEventArgs e)
    {
        ChaosHandler.RequestShuffle();
    }

    private void ShuffleOnStartToggle_OnClick(object sender, RoutedEventArgs e)
    {
        ChaosHandler.RequestShuffleOnStartToggle();
    }

    private void ForceMoveCollectibles_Click(object sender, RoutedEventArgs e)
    {
        Client.HChaos.MoveCollectibles(Client.HLevel.CurrentLevelId);
    }

    private void AbortSession_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HCommand.Commands["hs"].InitExecute(new []{"abort"});
    }

    private void OpenDrafts_OnClick(object sender, RoutedEventArgs e)
    {
        (DataContext as LobbyViewModel)?.OpenDrafts();
    }

    private void ResetScores_Click(object sender, RoutedEventArgs e)
    {
        CollectionModeHandler.ResetScores();
    }

    private void AbortDrafts_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HHideSeek.StopDraftsSession();
    }

    private void ChangeRole_OnClick(object sender, RoutedEventArgs e)
    {
        Client.HHideSeek.ForceChangeRole(clickedPlayer.Id);
    }

    private void AbortCollection_Click(object sender, RoutedEventArgs e)
    {
        CollectionModeHandler.RequestAbort();
    }

    private void ClearCLMLog_Click(object sender, RoutedEventArgs e)
    {
        CollectionModeHandler.ClearLogs();
    }
    
    private void SuggestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SuggestionsList.SelectedItem == null) 
            return;
        TextBoxInput.Text = SuggestionsList.SelectedItem.ToString() ?? string.Empty;
        TextBoxInput.CaretIndex = TextBoxInput.Text.Length; // Move caret to the end
        CommandCompletePopup.IsOpen = true;
    }

    private void TextBoxInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        var input = TextBoxInput.Text;
        if (!input.StartsWith('/'))
        {
            CommandCompletePopup.IsOpen = false;
            return;
        }
        input = input.Substring(1, input.Length - 1);
        if (!string.IsNullOrEmpty(input))
        {
            var suggestions = new List<string>();
            var parts = input.Split(" ");
            if (parts.Length == 1)
                suggestions = Client.HCommand.Commands.Keys
                    .Where(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    .Select(c => "/" + c) // Add the "/" prefix to each command
                    .ToList();
            else if (parts.Length > 1 && Client.HCommand.Commands.TryGetValue(parts[0], out var command))
                suggestions = command.Usages.Select(usage => $"/{parts[0]} {string.Join(" ", usage.Split(' ').Skip(1))}") // Add the "/" and the base command to each usage suggestion
                    .ToList();
            else
                suggestions = suggestions;

            if (suggestions.Any())
            {
                // Update the ListBox with suggestions
                SuggestionsList.ItemsSource = suggestions;
                CommandCompletePopup.IsOpen = true;
            }
            else
                CommandCompletePopup.IsOpen = false;
        }
        else
            CommandCompletePopup.IsOpen = false;
    }
    
}