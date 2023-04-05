using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void TyFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = BasicIoC.SetupViewModel.TyPath ?? dialog.SelectedPath;
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }
            if ((bool)dialog.ShowDialog(this))
            {
                BasicIoC.SetupViewModel.TyPath = dialog.SelectedPath;
            }
        }

        private void MTPFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = BasicIoC.SetupViewModel.MTPPath ?? dialog.SelectedPath;
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }
            if ((bool)dialog.ShowDialog(this))
            {
                BasicIoC.SetupViewModel.MTPPath = dialog.SelectedPath;
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            BasicIoC.SetupViewModel.Install();
        }
    }
}
