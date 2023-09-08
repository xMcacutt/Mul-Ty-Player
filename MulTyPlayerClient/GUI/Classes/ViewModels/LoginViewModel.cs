using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        public bool ConnectEnabled { get; set; } = true;

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
            ConnectEnabled = false;            
            model.Connect(ConnectingAddress, Name, Pass);
        }

        private void Model_OnLoginSuccess()
        {
            //display some message about successful login idgaf
        }

        private async void Model_OnLoginFailed()
        {
            ConnectEnabled = false;
            await Task.Delay(2000);
            ConnectEnabled = true;
        }

        public void OnEntered()
        {
        }

        public void OnExited()
        {
        }
    }
}
