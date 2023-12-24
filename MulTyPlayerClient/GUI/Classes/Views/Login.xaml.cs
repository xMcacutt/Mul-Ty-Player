using System.Windows.Controls;
using System.Windows.Input;
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
}