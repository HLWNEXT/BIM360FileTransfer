using BIM360FileTransfer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BIM360FileTransfer.Commands
{
    internal class LoadLocalFilesCommand : ICommand
    {
        public FileBrowseViewModel _fileBrowseViewModel;

        /// <summary>
        /// Initializes a new instance of the FileBrowseCommand class.
        /// </summary>
        /// <param name="viewModel">Main view model.</param>
        public LoadLocalFilesCommand(FileBrowseViewModel fileBrowseViewModel)
        {
            _fileBrowseViewModel = fileBrowseViewModel;
        }



        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {

            _fileBrowseViewModel.LoadLocalFile();
        }
        #endregion
    }
}
