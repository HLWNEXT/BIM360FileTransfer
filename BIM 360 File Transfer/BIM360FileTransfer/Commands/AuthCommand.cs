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
        /// Initializes a new instance of the AuthCommand class.
        /// </summary>
        /// <param name="viewModel">Main view model.</param>
        public AuthCommand(AuthViewModel authViewModel)
        {
            _AuthViewModel = authViewModel;
        }

        private AuthViewModel _AuthViewModel;

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _AuthViewModel.CanOpenAuthPage;
        }

        public void Execute(object parameter)
        {
            _AuthViewModel.OpenAuthPage();
        }
        #endregion
    }
}
