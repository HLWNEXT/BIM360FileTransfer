using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;
using System.Collections.ObjectModel;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    internal class ViewModel : BaseViewModel, IViewModel
    {
        public AuthViewModel AuthViewModel { get; private set; }
        public FileBrowseViewModel FileBrowseViewModel { get; private set; }


        public ViewModel()
        {
            this.AuthViewModel = new AuthViewModel();
            this.FileBrowseViewModel = new FileBrowseViewModel();
            //this.FileTransferViewModel = new FileTransferViewModel(FileBrowseViewModel);
        }
    }
}
