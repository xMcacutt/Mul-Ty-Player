using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.ViewModels
{
    public class UpdateViewCommand : ICommand
    {
        private MainViewModel viewModel;

        public UpdateViewCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;


        public bool CanExecute(object parameter)
        {
            return parameter is BaseViewModel.View;
        }

        public void Execute(object parameter)
        {
            if (parameter is BaseViewModel.View view)
            {
                viewModel.ChangeToView(view);
            }
        }
    }
}
