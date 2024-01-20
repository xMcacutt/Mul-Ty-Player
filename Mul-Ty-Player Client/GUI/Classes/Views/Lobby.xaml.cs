using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient.GUI.Views;

/// <summary>
///     Interaction logic for ClientGUI.xaml
/// </summary>
public partial class Lobby : UserControl
{
    private PlayerInfo clickedPlayer;

    public Lobby()
    {
        InitializeComponent();
    }

    private void TextboxInput_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Return:
                LobbyModel.ProcessInput(TextBoxInput.Text);
                TextBoxInput.Text = "";
                break;
            case Key.Up or Key.Down:
                var recallText = LobbyModel.ProcessRecall(e.Key == Key.Up);
                if (e.Key == Key.Up && string.IsNullOrWhiteSpace(recallText))
                    break;
                TextBoxInput.Text = recallText;
                TextBoxInput.CaretIndex = TextBoxInput.Text.Length;
                break;
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        App.SettingsWindow = new SettingsMenu();
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

    private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
    {
        TyProcess.TryLaunchGame();
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
        clickedPlayer = row.Item as PlayerInfo;
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
        message.AddUShort(clickedPlayer.ClientID);
        Client._client.Send(message);
    }

    private void KickPlayer_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        Client.HCommand.Commands["kick"].InitExecute(new string[] { clickedPlayer.ClientID.ToString() });
    }

    private void HostMenuButton_Click(object sender, RoutedEventArgs e)
    {
        // Open the context menu on left click
        if (sender is FrameworkElement element)
        {
            element.ContextMenu.IsOpen = true;
        }
    }

    private void LevelLockToggle_Click(object sender, RoutedEventArgs e)
    {
        var value = SettingsHandler.DoLevelLock ? "False" : "True";
        Client.HCommand.Commands["levellock"].InitExecute(new string[] { value });
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
        if (clickedPlayer.ClientID == Client._client.Id)
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
}