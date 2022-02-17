using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    public abstract class CategoryViewModel : SelectableObject, IModelCore<ICategory>
    {
        public ICategory Model { get; set; }

        public string CategoryProjectId => Model.ProjectId;
        public string CategoryName => Model.Name;
        public string CategoryType => Model.Type;
        public string CategoryId => Model.Id;

        private List<CategoryViewModel> children;
        public List<CategoryViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = CreateChildren();
                    if (children != null)
                        return children.Select(delegate (CategoryViewModel c)
                        {
                            //c.Parent = this;
                            c.Level = Level + 1;
                            return c;
                        }).ToList();
                    return null;
                }
                return children;
            }
            set
            {
                children = value;
                OnPropertyChanged();
            }
        }
        private int level;
        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                OnPropertyChanged();
            }
        }
        


        public CategoryViewModel Parent { get; set; }


        protected CategoryViewModel(ICategory category, ICommand command = null)
        {
            Model = category;
            if (Model is INotifyPropertyChanged model)
                model.PropertyChanged += Model_PropertyChanged;
            Children = null;
            Command = command;
        }

   

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

        


        protected abstract List<CategoryViewModel> CreateChildren();


        public ICommand Command { get; }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        protected override void OnSelectionChanged()
        {
            if (IsSelected)
            {
                //if (Parent != null)
                //{
                //    var children = Parent.Children;
                //    if (children != null)
                //        children.ForEach(x => { if (!x.Equals(this)) x.IsSelected = false; });
                //    Parent.OnSelectionChanged();
                //    OnPropertyChanged("Subjects");
                //}
            }
        }


    }
}
