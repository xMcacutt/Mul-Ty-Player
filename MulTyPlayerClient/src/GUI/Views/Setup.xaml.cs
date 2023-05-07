using MulTyPlayerClient.GUI.ViewModels;
using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MulTyPlayerClient.GUI.Views
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : UserControl
    {
        Window window;
        public Setup()
        {
            InitializeComponent();
            window = Window.GetWindow(this);
        }

        private void TyFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = MainViewModel.Setup.TyPath ?? dialog.SelectedPath;
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(window, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }
            if ((bool)dialog.ShowDialog(window))
            {
                MainViewModel.Setup.TyPath = dialog.SelectedPath;
            }
        }

        private void MTPFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = MainViewModel.Setup.MTPPath ?? dialog.SelectedPath;
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBox.Show(window, "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }
            if ((bool)dialog.ShowDialog(window))
            {
                MainViewModel.Setup.MTPPath = dialog.SelectedPath;
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Setup.Install();
        }
    }
}
