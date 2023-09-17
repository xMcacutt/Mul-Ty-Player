using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class LoginViewModel : IViewModel
    {
        public string Name { get; set; }
        public string Pass { get; set; }
        public string ConnectingAddress { get; set; }

        public bool HideName { get; set; }
        public bool HidePass { get; set; } = true;
        public bool HideAddress { get; set; } = true;

        public ICommand ConnectCommand { get; set; }
        public bool ConnectButtonEnabled { get; set; } = true;

        public bool ConnectionAttemptSuccessful { get; set; }
        public bool ConnectionAttemptCompleted  { get; set; }

        private LoginModel model;

        public LoginViewModel()
        {
            model = ModelController.Login;
            ConnectingAddress = model.GetIP();
            Name = model.GetName();
            Pass = model.GetPass();
            ConnectCommand = new RelayCommand(TryConnect);

            model.OnLoginFailed += Model_OnLoginFailed;
            model.OnLoginSuccess += Model_OnLoginSuccess;
            
        }

        private void TryConnect()
        {
            ConnectButtonEnabled = false;
            Debug.WriteLine("Connecting address: " + ConnectingAddress);
            Debug.WriteLine("Pass: " + Pass);
            model.Connect(ConnectingAddress, Name, Pass);
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

        private void Model_EnableLoginButton()
        {
            ConnectButtonEnabled = true;
        }

        public void OnEntered()
        {
            model.SetName();
            ConnectButtonEnabled = true;
        }

        public void OnExited()
        {
        }
    }
}
