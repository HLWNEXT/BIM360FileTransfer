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
using CefSharp.Wpf;
using CefSharp;
using Autodesk.Forge;
using BIM360FileTransfer.VIews;
using BIM360FileTransfer.Models;
using BIM360FileTransfer.Commands;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    internal class ViewModel : BaseViewModel, IViewModel
    {

        public ViewModel()
        {
            _AuthModel = new AuthModel("");
            OpenAuthCommand = new AuthCommand(this);
            FileBrowseCommand = new FileBrowseCommand(this);
        }


        public bool CanOpenAuthPage
        {
            get
            {
                if (AuthModel is null)
                {
                    return false;
                }
                return AuthModel.Code == "";
            }
        }

        private AuthModel _AuthModel;

        public AuthModel AuthModel
        {
            get { return _AuthModel; }
        }

        public ICommand OpenAuthCommand
        {
            get;
            private set;
        }

        public ICommand FileBrowseCommand
        {
            get;
            private set;
        }
    }
}
