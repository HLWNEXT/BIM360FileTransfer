using BIM360FileTransfer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BIM360FileTransfer.Commands
{
    internal class AuthCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the CustomerUpdateCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public AuthCommand(ViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        private ViewModel _ViewModel;

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanOpenAuthPage;
        }

        public void Execute(object parameter)
        {
            //_ViewModel.OpenAuthPage();
            AuthViewModel authViewModel = new AuthViewModel();
            authViewModel.OpenAuthPage();
        }
        #endregion
    }
}
