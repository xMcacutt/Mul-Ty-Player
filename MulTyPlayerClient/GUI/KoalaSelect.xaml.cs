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
    }
}
