using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MulTyPlayerClient.GUI.Classes.Views;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

/// <summary>
///     Interaction logic for Login.xaml
/// </summary>
public partial class Login : UserControl
{
    public Login()
    {
        InitializeComponent();
    }

    private void FieldHostIp_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            //((LoginViewModel)DataContext).ConnectCommand.Execute(e);
        }
    }

    private void FieldHostIp_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //if (FieldHostIp.SelectedItem != null)
            //(DataContext as LoginViewModel)!.SelectedServerChanged(FieldHostIp.SelectedItem);
    }

    private void ServerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not LoginViewModel vm)
            return;
        vm.IsPopupOpen = false;
        if (ServerListView.SelectedItem == null)
            return;
        vm.ConnectingAddress = ServerListView.SelectedItem.ToString();
    }
}