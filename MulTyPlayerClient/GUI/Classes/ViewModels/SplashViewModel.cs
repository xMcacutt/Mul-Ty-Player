﻿using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class SplashViewModel
    {
        public string MessageText { get; set; } = "Hello!";
        public Splash SplashWindow;

        public SplashViewModel()
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) => FindTy();
            backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                Login login = new();
                login.Show();
                BasicIoC.LoginViewModel.SetupLogin();
                SplashWindow.Close();
            };
            backgroundWorker.RunWorkerAsync();
        }

        private void FindTy()
        {
            Thread.Sleep(2000);
            var messageShown = false;
            while (ProcessHandler.FindTyProcess() == null)
            {
                if (!messageShown)
                {
                    BasicIoC.SplashScreenViewModel.MessageText = "Mul-Ty-Player could not be found.\nPlease open the game to continue.";
                    messageShown = true;
                }
            }
            BasicIoC.SplashScreenViewModel.MessageText = "Mul-Ty-Player is open!";
            Thread.Sleep(1000);
        }
    }
}