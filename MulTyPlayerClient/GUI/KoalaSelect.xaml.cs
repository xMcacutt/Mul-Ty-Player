using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MulTyPlayerClient.GUI
{
    /// <summary>
    /// Interaction logic for KoalaSelect.xaml
    /// </summary>
    public partial class KoalaSelect : Window
    {

        public KoalaSelect()
        {
            InitializeComponent();
            BoonieME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Boonie.mp4");
            MimME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Mim.mp4");
            KatieME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Katie.mp4");
            KikiME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Kiki.mp4");
            SnugsME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Snugs.mp4");
            DubboME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Dubbo.mp4");
            GummyME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Gummy.mp4");
            ElizabethME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Elizabeth.mp4");
        }

        private void Boonie_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Boonie = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Boonie.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Boonie.jpg";
        }

        private void Mim_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Mim = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Mim.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Mim.jpg";
        }

        private void Gummy_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Gummy = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Gummy.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Gummy.jpg";
        }

        private void Snugs_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Snugs = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Snugs.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Snugs.jpg";
        }

        private void Katie_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Katie = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Katie.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Katie.jpg";
        }

        private void Kiki_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Kiki = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Kiki.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Kiki.jpg";
        }

        private void Elizabeth_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Elizabeth = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Elizabeth.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Elizabeth.jpg";
        }

        private void Dubbo_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BasicIoC.KoalaSelectViewModel.Dubbo = (bool)e.NewValue ? @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/Dubbo.png" : @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Dubbo.jpg";
        }

        private void BoonieClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.BoonieAvailable)
            {
                BoonieME.Position = TimeSpan.Zero;
                BoonieME.Visibility = Visibility.Visible;
                BoonieME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Boonie");
            }
        }

        private void MimClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.MimAvailable)
            {
                BoonieME.Position = TimeSpan.Zero;
                MimME.Visibility = Visibility.Visible;
                MimME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Mim");
            }
        }

        private void KatieClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.KatieAvailable)
            {
                KatieME.Position = TimeSpan.Zero;
                KatieME.Visibility = Visibility.Visible;
                KatieME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Katie");
            }
        }

        private void KikiClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.KikiAvailable)
            {
                KikiME.Position = TimeSpan.Zero;
                KikiME.Visibility = Visibility.Visible;
                KikiME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Kiki");
            }
        }

        private void SnugsClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.SnugsAvailable)
            {
                SnugsME.Position = TimeSpan.Zero;
                SnugsME.Visibility = Visibility.Visible;
                SnugsME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Snugs");
            }
        }

        private void DubboClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.DubboAvailable)
            {
                DubboME.Position = TimeSpan.Zero;
                DubboME.Visibility = Visibility.Visible;
                DubboME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Dubbo");
            }
        }

        private void GummyClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.GummyAvailable)
            {
                GummyME.Position = TimeSpan.Zero;
                GummyME.Visibility = Visibility.Visible;
                GummyME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Gummy");
            }
        }

        private void ElizabethClicked(object sender, MouseButtonEventArgs e)
        {
            if (BasicIoC.KoalaSelectViewModel.ElizabethAvailable)
            {
                ElizabethME.Position = TimeSpan.Zero;
                ElizabethME.Visibility = Visibility.Visible;
                ElizabethME.Play();
                BasicIoC.KoalaSelectViewModel.KoalaClicked("Elizabeth");
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            BoonieME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Boonie.mp4");
            MimME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Mim.mp4");
            KatieME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Katie.mp4");
            KikiME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Kiki.mp4");
            SnugsME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Snugs.mp4");
            DubboME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Dubbo.mp4");
            GummyME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Gummy.mp4");
            ElizabethME.Source = new Uri(@"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/Elizabeth.mp4");
            BoonieME.Visibility = Visibility.Collapsed;
            MimME.Visibility = Visibility.Collapsed;
            KatieME.Visibility = Visibility.Collapsed;
            KikiME.Visibility = Visibility.Collapsed;
            SnugsME.Visibility = Visibility.Collapsed;
            DubboME.Visibility = Visibility.Collapsed;
            GummyME.Visibility = Visibility.Collapsed;
            ElizabethME.Visibility = Visibility.Collapsed;
        }
    }
}
