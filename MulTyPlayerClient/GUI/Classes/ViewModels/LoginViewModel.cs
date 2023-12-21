using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class LoginViewModel : IViewModel
{
    public LoginViewModel()
    {
        ConnectCommand = new RelayCommand(TryConnect);
        Login.OnLoginFailed += Model_OnLoginFailed;
        Login.OnLoginSuccess += Model_OnLoginSuccess;
    }

    public string Name { get; set; }
    public string Pass { get; set; }
    public string ConnectingAddress { get; set; }

    public bool HideName { get; set; }
    public bool HidePass { get; set; } = true;
    public bool HideAddress { get; set; } = true;

    public ICommand ConnectCommand { get; set; }
    public bool ConnectButtonEnabled { get; set; } = true;

    public bool ConnectionAttemptSuccessful { get; set; }
    public bool ConnectionAttemptCompleted { get; set; }

    private static LoginModel Login => ModelController.Login;

    public void OnEntered()
    {
        Login.Setup();
        ConnectingAddress = Login.GetIP();
        Name = Login.GetName();
        Pass = Login.GetPass();
        ConnectButtonEnabled = true;
    }

    public void OnExited()
    {
    }

    private void TryConnect()
    {
        ConnectButtonEnabled = false;
        Debug.WriteLine("Connecting address: " + ConnectingAddress);
        Debug.WriteLine("Pass: " + Pass);
        Login.Connect(ConnectingAddress, Name, Pass);
    }

    private void Model_OnLoginSuccess()
    {
        //display some message about successful login idk
    }

    private async void Model_OnLoginFailed()
    {
        await Task.Delay(1000);
        ConnectButtonEnabled = true;
    }
}