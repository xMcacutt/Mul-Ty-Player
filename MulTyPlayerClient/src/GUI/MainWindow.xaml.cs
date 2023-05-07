using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Input;
using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.GUI.Views;

namespace MulTyPlayerClient.GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
