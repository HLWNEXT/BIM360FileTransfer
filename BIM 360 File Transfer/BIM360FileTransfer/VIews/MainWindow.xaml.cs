using BIM360FileTransfer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BIM360FileTransfer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

		private void TheTreeView_PreviewSelectionChanged(object sender, PreviewSelectionChangedEventArgs e)
		{
			// Selection is not locked, apply other conditions.
			// Require all selected items to be of the same type. If an item of another data
			// type is already selected, don't include this new item in the selection.
			if (e.Selecting && SourceCategoryTree.SelectedItems.Count > 0)
			{
				e.CancelThis = e.Item.GetType() != SourceCategoryTree.SelectedItems[0].GetType();
			}
		}
		private void TargetTreeView_PreviewSelectionChanged(object sender, PreviewSelectionChangedEventArgs e)
		{
			// Selection is not locked, apply other conditions.
			// Require all selected items to be of the same type. If an item of another data
			// type is already selected, don't include this new item in the selection.
			if (e.Selecting && TargetCategoryTree.SelectedItems.Count > 0)
			{
				e.CancelThis = e.Item.GetType() != TargetCategoryTree.SelectedItems[0].GetType();
			}
		}
	}
}
