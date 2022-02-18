using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    public abstract class CategoryViewModel : INotifyPropertyChanged
    {
        #region Data
        public ICategory Model { get; set; }

        public string CategoryProjectId => Model.ProjectId;
        public string CategoryName => Model.Name;
        public string CategoryType => Model.Type;
        public string CategoryId => Model.Id;

        private readonly ObservableCollection<CategoryViewModel> children;
        //public readonly CategoryViewModel parent;

        private int level;
        

        private bool isSelected;
        private bool isVisible = true;
        private string remarks;
        #endregion



        #region Constructor
        protected CategoryViewModel(ICategory category, ICommand command = null)
        {
            Model = category;
            if (Model is INotifyPropertyChanged model)
                model.PropertyChanged += Model_PropertyChanged;
            //this.parent = parent;
            children = new ObservableCollection<CategoryViewModel>();
            //Command = command;
        }
        #endregion

        #region Public Properties
        /// <summary>
		/// Returns the logical child items of this object.
		/// </summary>
		public ObservableCollection<CategoryViewModel> Children
        {
            get { return children; }
        }

        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                OnPropertyChanged("Level");
            }
        }

        /// <summary>
		/// Gets/sets whether the TreeViewItem 
		/// associated with this object is selected.
		/// </summary>
		public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
        #endregion



        public override bool Equals(object obj)
        {
            CategoryViewModel categoryCore;
            return (categoryCore = obj as CategoryViewModel) != null && CategoryId.Equals(categoryCore.CategoryId);
        }
        public override int GetHashCode()
        {
            if (CategoryId != null) return CategoryId.GetHashCode();
            return int.MinValue;
        }

        


        //protected abstract ObservableCollection<CategoryViewModel> CreateChildren();


        public ICommand Command { get; }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        //protected override void OnSelectionChanged()
        //{
        //    if (IsSelected)
        //    {
        //        //if (Parent != null)
        //        //{
        //        //    var children = Parent.Children;
        //        //    if (children != null)
        //        //        children.ForEach(x => { if (!x.Equals(this)) x.IsSelected = false; });
        //        //    Parent.OnSelectionChanged();
        //        //    OnPropertyChanged("Subjects");
        //        //}
        //    }
        //}

        #region INotifyPropertyChanged members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged members


    }
}
