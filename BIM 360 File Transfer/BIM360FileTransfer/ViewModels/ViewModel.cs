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
        //ObservableCollection<object> _children;
        //public ObservableCollection<object> Children { get { return _children; } }
        public AuthViewModel AuthViewModel { get; private set; }
        public FileBrowseViewModel FileBrowseViewModel { get; private set; }

        //private List<CategoryViewModel> categoryTree;
        //public List<CategoryViewModel> CategoryTree
        //{
        //    get { return categoryTree; }
        //    set
        //    {
        //        categoryTree = value;
        //        OnPropertyChanged("CategoryTree");
        //    }
        //}

        public ViewModel()
        {
            this.AuthViewModel = new AuthViewModel();
            this.FileBrowseViewModel = new FileBrowseViewModel();

            //_children = new ObservableCollection<object>();
            //_children.Add(new AuthCommand(this));
            //_children.Add(new FileBrowseCommand(this));


            //OpenAuthCommand = new AuthCommand(this);
            //FileBrowseCommand = new FileBrowseCommand(this);
        }


        
    }
}
