using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.Networking;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.Views
{
    /// <summary>
    /// Interaction logic for KoalaSelect.xaml
    /// </summary>
    public partial class KoalaSelect : UserControl
    {
        public KoalaSelect()
        {
            InitializeComponent();
            //BoonieME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Boonie.mp4");
            MimME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Mim.mp4");
            KatieME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Katie.mp4");
            KikiME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Kiki.mp4");
            SnugsME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Snugs.mp4");
            DubboME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Dubbo.mp4");
            GummyME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Gummy.mp4");
            ElizabethME.Source = new Uri(@"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Selected/Elizabeth.mp4");
            //BoonieME.Visibility = Visibility.Collapsed;
            MimME.Visibility = Visibility.Collapsed;
            KatieME.Visibility = Visibility.Collapsed;
            KikiME.Visibility = Visibility.Collapsed;
            SnugsME.Visibility = Visibility.Collapsed;
            DubboME.Visibility = Visibility.Collapsed;
            GummyME.Visibility = Visibility.Collapsed;
            ElizabethME.Visibility = Visibility.Collapsed;

            vm = (KoalasViewModel)DataContext;
        }

        KoalasViewModel vm;

        private void BoonieClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.BoonieAvailable)
            {
                //BoonieME.Position = TimeSpan.Zero;
                //BoonieME.Visibility = Visibility.Visible;
                //BoonieME.Play();
                vm.OnKoalaClicked("Boonie");
            }
        }

        private void MimClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.MimAvailable)
            {
                //MimME.Position = TimeSpan.Zero;
                MimME.Visibility = Visibility.Visible;
                MimME.Play();
                vm.OnKoalaClicked("Mim");
            }
        }

        private void KatieClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.KatieAvailable)
            {
                KatieME.Position = TimeSpan.Zero;
                KatieME.Visibility = Visibility.Visible;
                KatieME.Play();
                vm.OnKoalaClicked("Katie");
            }
        }

        private void KikiClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.KikiAvailable)
            {
                KikiME.Position = TimeSpan.Zero;
                KikiME.Visibility = Visibility.Visible;
                KikiME.Play();
                vm.OnKoalaClicked("Kiki");
            }
        }

        private void SnugsClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.SnugsAvailable)
            {
                SnugsME.Position = TimeSpan.Zero;
                SnugsME.Visibility = Visibility.Visible;
                SnugsME.Play();
                vm.OnKoalaClicked("Snugs");
            }
        }

        private void DubboClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.DubboAvailable)
            {
                DubboME.Position = TimeSpan.Zero;
                DubboME.Visibility = Visibility.Visible;
                DubboME.Play();
                vm.OnKoalaClicked("Dubbo");
            }
        }

        private void GummyClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.GummyAvailable)
            {
                GummyME.Position = TimeSpan.Zero;
                GummyME.Visibility = Visibility.Visible;
                GummyME.Play();
                vm.OnKoalaClicked("Gummy");
            }
        }

        private void ElizabethClicked(object sender, MouseButtonEventArgs e)
        {
            if (vm.ElizabethAvailable)
            {
                ElizabethME.Position = TimeSpan.Zero;
                ElizabethME.Visibility = Visibility.Visible;
                ElizabethME.Play();
                vm.OnKoalaClicked("Elizabeth");
            }
        }
    }
}
