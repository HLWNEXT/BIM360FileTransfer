using BIM_360_File_Transfer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BIM_360_File_Transfer.Commands
{
    internal class CustomerUpdateCommand : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the CustomerUpdateCommand class.
        /// </summary>
        /// <param name="viewModel"></param>
        public CustomerUpdateCommand(CustomerViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        private CustomerViewModel _ViewModel;

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;}
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanUpdate;        
        }

        public void Execute(object parameter)
        {
            _ViewModel.SaveChanges();
        }
        #endregion
    }
}
