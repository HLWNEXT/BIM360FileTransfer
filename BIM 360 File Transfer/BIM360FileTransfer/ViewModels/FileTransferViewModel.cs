using BIM360FileTransfer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.ViewModels
{
    internal class FileTransferViewModel : BaseViewModel, IViewModel
    {
        private FileBrowseViewModel fileBrowseViewModel;

        public FileTransferViewModel(FileBrowseViewModel fileBrowseViewModel)
        {
            this.fileBrowseViewModel = fileBrowseViewModel;
        }

    }
}
