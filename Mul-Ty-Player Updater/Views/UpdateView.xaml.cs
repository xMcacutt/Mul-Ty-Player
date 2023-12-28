using System;
using System.Windows;
using Mul_Ty_Player_Updater.ViewModels;

namespace Mul_Ty_Player_Updater.Views;

public partial class UpdateView : Window
{
    public UpdateView()
    {
        InitializeComponent();
    }

    private void UpdateView_OnRendered(object? sender, EventArgs e)
    {
        if (DataContext is not UpdateViewModel vm) return;
        vm.UpdateCompleted += VmOnUpdateCompleted;
        vm.GetUpdate();
    }

    private void VmOnUpdateCompleted(object? sender, object e)
    {
        if (DataContext is UpdateViewModel vm)
            vm.UpdateCompleted -= VmOnUpdateCompleted;
        DialogResult = true;
    }
}
