using MulTyPlayerClient.GUI.Views;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Settings;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SplashViewModel : BaseViewModel
    {
        public string MessageText { get => model.MessageText; set => model.MessageText = value; }
        public Visibility LaunchGameButtonVisibility { get => model.LaunchGameButtonVisibility; set => model.LaunchGameButtonVisibility = value; }
        public bool EnableLaunchGameButton { get => model.EnableLaunchGameButton; set => model.EnableLaunchGameButton = value; }

        public event EventHandler Finished;

        SplashModel model;

        public SplashViewModel(SplashModel model) : base()
        {
            this.model = model;

            ViewModelEntered += (o, e) =>
            {
                var backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += (s, e) => model.InitializeTyProcess();
                backgroundWorker.RunWorkerCompleted += (s, e) =>
                {
                    MoveToView(View.Login);
                };
                backgroundWorker.RunWorkerAsync();
            };
        }
    }
}
