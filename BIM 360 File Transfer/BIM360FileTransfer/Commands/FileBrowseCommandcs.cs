using BIM360FileTransfer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BIM360FileTransfer.Commands
{
    internal class FileBrowseCommand : ICommand
    {
        private ViewModel _ViewModel;
        public FileBrowseViewModel fileBrowseViewModel;
        public IList<CategoryViewModel> result;

        /// <summary>
        /// Initializes a new instance of the FileBrowseCommand class.
        /// </summary>
        /// <param name="viewModel">Main view model.</param>
        public FileBrowseCommand(ViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        

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
            fileBrowseViewModel = new FileBrowseViewModel();
            result = fileBrowseViewModel.GetCategoryAsync();
        }
        #endregion
    }
}
