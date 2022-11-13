using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BIM360FileTransfer.ViewModels;

namespace BIM360FileTransfer.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow ()
		{
			var viewModel = new ViewModel();
            DataContext = viewModel;
            InitializeComponent ();
            AppWindow = this;
		}

        public static MainWindow AppWindow
        {
            get;
            private set;
        }
    }
}