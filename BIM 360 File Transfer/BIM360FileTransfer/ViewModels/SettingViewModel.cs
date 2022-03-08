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
    internal class SettingViewModel : BaseViewModel, IViewModel
    {
        public SettingWindow SettingWindow;
        public FileBrowseViewModel fileBrowseViewModel;


        public SettingViewModel(FileBrowseViewModel fileBrowseViewModel)
        {
            this.fileBrowseViewModel = fileBrowseViewModel;
        }

        public void myFirstCommand(string par)
        {
            Console.Beep();
        }


        /// <summary>
        /// Open an setting window when initiate the transfer.
        /// </summary>
        public void OpenSettingWindow()
        {

            // Open the authentication window.
            SettingWindow = new SettingWindow();
            SettingWindow.Show();

        }



        public bool CanOpenSettingPage
        {
            get
            {
                return true;
            }
        }

        public ICommand OpenSettingCommand
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
