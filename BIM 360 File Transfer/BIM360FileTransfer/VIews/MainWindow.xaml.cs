using BIM360FileTransfer.ViewModels;
using System.Windows;
using System.Windows.Controls;


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
