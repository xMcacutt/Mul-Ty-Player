using PropertyChanged;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageViewModel
    {
        public string Name { get; set; }
        public SecureString Pass { get; set; }

        public LoginPageViewModel()
        {

        }

        public void SetupLogin()
        {
            SteamClient.Init(411960);
            if (SteamClient.IsValid) Name = SteamClient.Name;
            else Name = "Admin";
            SteamClient.Shutdown();
        }

    }
}
