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
using Autodesk.Forge.Model;
using Newtonsoft.Json;
using System.IO;

namespace BIM360FileTransfer.ViewModels
{
    internal class FileBrowseViewModel : BaseViewModel, IViewModel
    {
        private readonly List<SourceViewModel> entities;
        private IList<CategoryViewModel> categoryTree;

        public IList<CategoryViewModel> CategoryTree
        {
            get { return categoryTree; }
            set
            {
                categoryTree = value;
                OnPropertyChanged("CategoryTree");
            }
        }

        private ObservableCollection<SourceViewModel> items;

        public ObservableCollection<SourceViewModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        public FileBrowseViewModel()
        {
            FileBrowseCommand = new FileBrowseCommand(this);
            FileLoadCommand = new FileLoadCommand(this);
        }

        //private CategoryViewModel selectedCategory;

        //public CategoryViewModel SelectedCategory
        //{
        //    get { return selectedCategory; }
        //    set
        //    {
        //        if (!object.Equals(selectedCategory, value))
        //        {
        //            selectedCategory = value;
        //            if (selectedCategory != null && !selectedCategory.IsSelected)
        //                selectedCategory.IsSelected = true;
        //            OnPropertyChanged("SelectedCategory");
        //        }
        //        Items = new ObservableCollection<SourceViewModel>();
        //        foreach (var item in entities)
        //        {
        //            if (selectedCategory.Model.Subjects.Contains(item.Model.Name))
        //                Items.Add(item);
        //        }

        //    }
        //}

        internal void GetCategoryLocal()
        {
            using (StreamReader file = File.OpenText(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "\\Resources\\category.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.Formatting = Formatting.Indented;
                CategoryTree = (IList<CategoryViewModel>)serializer.Deserialize(file, typeof(IList<CategoryViewModel>));
            }
        }

        public void GetCategoryAsync()
        {
            var hubId = GetHub();
            CategoryTree = GetCategoryTree(hubId);
            SaveCategory(CategoryTree);
        }

        private void SaveCategory(IList<CategoryViewModel> categoryTree)
        {
            using (StreamWriter file = File.CreateText(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "\\Resources\\category.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Serialize(file, categoryTree);
            }
        }

        /// <summary>
        /// Get the working hub.
        /// </summary>
        public string GetHub()
        {
            HubsApi hubsAPIInstance = new HubsApi();
            hubsAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            var response = hubsAPIInstance.GetHubs();
            return response.data[0].id;
        }

        private ObservableCollection<CategoryViewModel> GetCategoryTree(string hubId)
        {
            var categoryTree = new ObservableCollection<CategoryViewModel> { GetProjects(hubId) };
            return categoryTree;
        }

        private CategoryViewModel GetProjects(string hubId)
        {
            var root = new CategoryModel("Projects", "root");
            var rootCategory = new PublicCategoryCore(root);

            ProjectsApi projectsAPIInstance = new ProjectsApi();
            projectsAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;

            var response = projectsAPIInstance.GetHubProjects(hubId);

            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
            {
                var type = objInfo.Value.type;
                var projectId = objInfo.Value.id;
                var rootFolderId = objInfo.Value.relationships.rootFolder.data.id;
                var name = objInfo.Value.attributes.name;

                var entity = new CategoryModel(rootFolderId, projectId, name, type);
                var thisCategory = new PublicCategoryCore(entity);
                //thisCategory.Parent = rootCategory;
                GetChildrenCategory(hubId, thisCategory);
                rootCategory.Children.Add(thisCategory);
            }

            
            return rootCategory;
        }

        private void GetChildrenCategory(string hubId, CategoryViewModel rootCategory)
        {
            
            if (rootCategory.CategoryType == "projects")
            {
                var apiInstance = new FoldersApi();
                var response = apiInstance.GetFolderContents(rootCategory.CategoryProjectId, rootCategory.CategoryId);
                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
                {
                    var type = objInfo.Value.type;
                    var folderId = objInfo.Value.id;
                    var name = objInfo.Value.attributes.name;

                    if (name == "Plans" || name == "Project Files")
                    {
                        var entity = new CategoryModel(folderId, rootCategory.CategoryProjectId, name, type);
                        var thisCategory = new PublicCategoryCore(entity);
                        //thisCategory.Parent = rootCategory;
                        GetChildrenCategory(hubId, thisCategory);
                        rootCategory.Children.Add(thisCategory);
                    }
                }
            }
            else if (rootCategory.CategoryType == "folders")
            {
                if (rootCategory.CategoryName == "Plans" || rootCategory.CategoryName == "Revit Upgrade Report")
                {
                    return;
                }
                var apiInstance = new FoldersApi();
                var response = apiInstance.GetFolderContents(rootCategory.CategoryProjectId, rootCategory.CategoryId);


                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
                {
                    var type = objInfo.Value.type;

                    if (type == "items")
                    {
                        foreach (KeyValuePair<string, dynamic> storageObjInfo in new DynamicDictionaryItems(response.included))
                        {
                            var new_type = storageObjInfo.Value.type;

                            if (new_type == "versions")
                            {
                                var id = storageObjInfo.Value.relationships.storage.data.id;
                                var storage_object_id = id.Substring(id.LastIndexOf('/') + 1);// id.Split("/")[-1];
                                //var bucket_id = id.split("/")[0].Split(":").slice(-1)[0];
                                var name = storageObjInfo.Value.attributes.displayName + " v" + storageObjInfo.Value.attributes.versionNumber.ToString();

                                var entity = new CategoryModel(storage_object_id, rootCategory.CategoryProjectId, name, new_type);
                                var thisCategory = new PublicCategoryCore(entity);
                                thisCategory.IsVisible = false;
                                //thisCategory.Parent = rootCategory;
                                rootCategory.Children.Add(thisCategory);
                            }
                        }
                    }
                    else 
                    {
                        var folderId = objInfo.Value.id;
                        var name = objInfo.Value.attributes.name;

                        var entity = new CategoryModel(folderId, rootCategory.CategoryProjectId, name, type);
                        var thisCategory = new PublicCategoryCore(entity);
                        //thisCategory.Parent = rootCategory;
                        GetChildrenCategory(hubId, thisCategory);
                        rootCategory.Children.Add(thisCategory);
                    }
                    
                }
            }
        }

        public bool CanFileBrowse
        {
            get
            {
                return true;
            }
        }

        public ICommand OpenFileBrowseCommand
        {
            get;
            private set;
        }

        public ICommand FileBrowseCommand
        {
            get;
            private set;
        }

        public ICommand OpenFileLoadCommand
        {
            get;
            private set;
        }

        public ICommand FileLoadCommand
        {
            get;
            private set;
        }
    }
}
