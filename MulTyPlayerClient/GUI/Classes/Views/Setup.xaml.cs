using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MulTyPlayerClient.GUI.Models;
using Ookii.Dialogs.Wpf;

namespace MulTyPlayerClient.GUI.Views;

/// <summary>
///     Interaction logic for Setup.xaml
/// </summary>
public partial class Setup : Window
{
    public Setup()
    {
        InitializeComponent();
        DataContext = ModelController.Setup;
    }

    private void TyFolderBrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
        dialog.ShowNewFolderButton = true;
        dialog.SelectedPath = ModelController.Setup.TyPath ?? dialog.SelectedPath;
        dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
        if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            MessageBox.Show(this,
                "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.",
                "Sample folder browser dialog");
        if ((bool)dialog.ShowDialog(this)) ModelController.Setup.TyPath = dialog.SelectedPath;
    }

    private void MTPFolderBrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.SelectedPaths = Array.Empty<string>();
        dialog.Multiselect = false;
        dialog.UseDescriptionForTitle = false;
        dialog.ShowNewFolderButton = false;
        dialog.SelectedPath = null;
        dialog.RootFolder = Environment.SpecialFolder.Desktop;
        dialog.Description = null;
        dialog.Description = "Select Ty the Tasmanian Tiger Folder.";
        dialog.ShowNewFolderButton = true;
        dialog.SelectedPath = ModelController.Setup.MTPPath ?? dialog.SelectedPath;
        dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
        if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            MessageBox.Show(this,
                "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.",
                "Sample folder browser dialog");
        if ((bool)dialog.ShowDialog(this)) ModelController.Setup.MTPPath = dialog.SelectedPath;
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        InstallProgressBar.Visibility = Visibility.Visible;
        ModelController.Setup.Install();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        WindowHandler.SetupWindow.Hide();
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();    
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}