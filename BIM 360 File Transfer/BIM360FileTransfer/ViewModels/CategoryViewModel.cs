using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Forge;
using Autodesk.Forge.Model;
using BIM360FileTransfer.Interfaces;
using BIM360FileTransfer.Models;

namespace BIM360FileTransfer.ViewModels
{
    [Serializable]
    public abstract class CategoryViewModel : BaseViewModel, IViewModel, ICloneable
    {
        #region Data
        public ICategory Model { get; set; }
        public string CategoryProjectId => Model.ProjectId;
        public string CategoryName => Model.Name;
        public string CategoryType => Model.Type;
        public string CategoryId => Model.Id;
        public string CategoryBucketId => Model.BucketId;
        public string CategoryPath;

        private readonly ObservableCollection<CategoryViewModel> children;
        //public readonly CategoryViewModel parent;

        private int level;
        

        private bool isSelected;
        private bool isVisible = true;
        private bool isVisibleInSource = true;
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

        public string Path
        {
            get { return CategoryPath; }
            set
            {
                CategoryPath = value;
                OnPropertyChanged("CategoryPath");
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
                    NotifyPropertyChanged("IsSelected");
                    OnPropertyChanged("IsSelected");
                }

                if (CategoryType == "projects" && value is true)
                {
                    _ = GetChildrenAsync();
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
                    NotifyPropertyChanged("IsSelected");
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsVisibleInSource
        {
            get { return isVisibleInSource; }
            set
            {
                if (value != isVisibleInSource)
                {
                    isVisibleInSource = value;
                    NotifyPropertyChanged("IsVisibleInSource");
                    OnPropertyChanged("IsVisibleInSource");
                }
            }
        }
        #endregion

        #region Get Children

        private async Task GetChildrenAsync()
        {
            var folderAPIInstance = new FoldersApi();
            folderAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            var response = folderAPIInstance.GetFolderContents(CategoryProjectId, CategoryId);
            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
            {
                var type = objInfo.Value.type;
                var folderId = objInfo.Value.id;
                var name = objInfo.Value.attributes.name;

                if (name == "Plans" || name == "Project Files")
                {
                    var entity = new CategoryModel(folderId, CategoryProjectId, name, type);
                    var thisCategory = new PublicCategoryCore(entity);
                    thisCategory.CategoryPath = CategoryPath + "//" + name;
                    //thisCategory.Parent = rootCategory;
                    await Task.Run(() => GetChildrenCategoryAsync(thisCategory));
                    Children.Add(thisCategory);
                }
            }
        }

        private async Task GetChildrenCategoryAsync(CategoryViewModel rootCategory)
        {
            if (rootCategory.CategoryName == "Plans" || rootCategory.CategoryName == "Revit Upgrade Report")
            {
                return;
            }
            var apiInstance = new FoldersApi();
            var response = apiInstance.GetFolderContents(rootCategory.CategoryProjectId, rootCategory.CategoryId);

            bool isItemExist = false;

            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
            {
                var type = objInfo.Value.type;
                if (type == "items")
                {
                    isItemExist = true;
                    var itemId = objInfo.Value.id;
                    var name = objInfo.Value.attributes.displayName;

                    var entity = new CategoryModel(itemId, rootCategory.CategoryProjectId, name, type);
                    var thisCategory = new PublicCategoryCore(entity);
                    thisCategory.CategoryPath = rootCategory.CategoryPath + "//" + name;
                    thisCategory.isVisibleInSource = false;
                    rootCategory.Children.Add(thisCategory);
                    continue;
                }
                else
                {
                    var folderId = objInfo.Value.id;
                    var name = objInfo.Value.attributes.name;

                    var entity = new CategoryModel(folderId, rootCategory.CategoryProjectId, name, type);
                    var thisCategory = new PublicCategoryCore(entity);
                    thisCategory.CategoryPath = rootCategory.CategoryPath + "//" + name;
                    //thisCategory.Parent = rootCategory;
                    await Task.Run(() => GetChildrenCategoryAsync(thisCategory));
                    rootCategory.Children.Add(thisCategory);
                }
            }
            if (isItemExist)
            {
                foreach (KeyValuePair<string, dynamic> storageObjInfo in new DynamicDictionaryItems(response.included))
                {
                    var new_type = storageObjInfo.Value.type;
                    if (new_type == "versions")
                    {
                        var id = storageObjInfo.Value.relationships.storage.data.id;
                        var storage_object_id = id.Substring(id.LastIndexOf('/') + 1);
                        var bucket_id = id.Substring(0, id.LastIndexOf('/')).Substring(id.Substring(0, id.LastIndexOf('/') + 1).LastIndexOf(':') + 1);
                        var name = storageObjInfo.Value.attributes.displayName + " v" + storageObjInfo.Value.attributes.versionNumber.ToString();

                        var entity = new CategoryModel(storage_object_id, bucket_id, rootCategory.CategoryProjectId, name, new_type);
                        var thisCategory = new PublicCategoryCore(entity);
                        thisCategory.IsVisible = false;
                        thisCategory.CategoryPath = rootCategory.CategoryPath + "//" + name;
                        //thisCategory.Parent = rootCategory;
                        rootCategory.Children.Add(thisCategory);
                    }
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

        


        public ICommand Command { get; }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public object Clone()
        {
            return (CategoryViewModel)base.MemberwiseClone();
        }



        #region INotifyPropertyChanged members


        #endregion INotifyPropertyChanged members


    }
}
