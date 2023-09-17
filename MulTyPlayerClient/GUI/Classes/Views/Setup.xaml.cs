using MulTyPlayerClient.GUI.Models;
using Ookii.Dialogs.Wpf;
using System.Windows;

namespace MulTyPlayerClient.GUI.Views
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
            dialog.SelectedPath = ModelController.Setup.TyPath ?? dialog.SelectedPath;
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }
            if ((bool)dialog.ShowDialog(this))
            {
                ModelController.Setup.TyPath = dialog.SelectedPath;
            }
        }

        private void MTPFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = ModelController.Setup.MTPPath ?? dialog.SelectedPath;
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(this, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }
            if ((bool)dialog.ShowDialog(this))
            {
                ModelController.Setup.MTPPath = dialog.SelectedPath;
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            ModelController.Setup.Install();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            WindowHandler.SetupWindow.Hide();
        }
    }
}
