using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        if (e.Key == Key.Return && e.IsDown)
        {
            LobbyModel.ProcessInput(TextBoxInput.Text);
            TextBoxInput.Text = "";
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        WindowHandler.SettingsWindow = new SettingsMenu();
        ModelController.Settings.SetPropertiesFromSettings();
        WindowHandler.SettingsWindow.ShowDialog();
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta != 0)
            e = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta * -1);
        base.OnPreviewMouseWheel(e);
    }

    private void ReadyButton_Click(object sender, RoutedEventArgs e)
    {
        Client.HCommand.SetReady();
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

    private void Lobby_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        // Close the popup if any element outside the menu or button is clicked
        if (!IsMouseOverDataGrid(e.OriginalSource as DependencyObject)) PlayerInfo.SelectedItem = null;

        if (!IsMouseOverPopup(e.OriginalSource as DependencyObject)) GiveHostPopup.IsOpen = false;
    }

    private bool IsMouseOverDataGrid(DependencyObject element)
    {
        // Check if the mouse is over the button or popup
        return element != null &&
               element == PlayerInfo;
    }

    private bool IsMouseOverPopup(DependencyObject element)
    {
        // Check if the mouse is over the button or popup
        return element != null &&
               element == GiveHostPopup;
    }

    private void UIElement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var row = FindAncestor<DataGridRow>((DependencyObject)e.OriginalSource);
        if (row is null) return;
        GiveHostPopup.PlacementTarget = row;
        clickedPlayer = row.Item as PlayerInfo;
        if (PlayerHandler.Players[Client._client.Id].IsHost && !(row.Item as PlayerInfo).IsHost)
            GiveHostPopup.IsOpen = true;
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
}