﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mul_Ty_Player_Updater.ViewModels;
using Ookii.Dialogs.Wpf;

namespace Mul_Ty_Player_Updater.Views;

public partial class SetupView : Window
{
    public SetupView()
    {
        InitializeComponent();
    }
    
    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }
    
    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void MTPFolderBrowseButton_Click(object sender, RoutedEventArgs e)
    {
        ShowBrowseDialog(MTPPathBox, "Select Folder for MTP Game Folder.");
    }
    
    private void ClientFolderBrowseButton_Click(object sender, RoutedEventArgs e)
    {
        ShowBrowseDialog(ClientPathBox, "Select Folder for MTP Client Folder.");
    }
    
    private void ServerFolderBrowseButton_Click(object sender, RoutedEventArgs e)
    {
        ShowBrowseDialog(ServerPathBox, "Select Folder for MTP Server Folder.");
    }
    
    private void ShowBrowseDialog(TextBox textBox, string title)
    {
        var dialog = new VistaFolderBrowserDialog
        {
            Multiselect = false,
            UseDescriptionForTitle = true,
            RootFolder = Environment.SpecialFolder.Desktop,
            Description = title,
            ShowNewFolderButton = true,
            SelectedPath = Path.Exists(textBox.Text) ? textBox.Text : null,
        };
        if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            MessageBox.Show(this,
                "Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.",
                "Sample folder browser dialog");
        if ((bool)dialog.ShowDialog(this)!) textBox.Text = dialog.SelectedPath;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not SetupViewModel vm) return;
        vm.SaveSettings();
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}