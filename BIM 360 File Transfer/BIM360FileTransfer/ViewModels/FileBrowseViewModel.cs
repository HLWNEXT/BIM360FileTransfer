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
using System.Runtime.Serialization.Formatters.Binary;
using BIM360FileTransfer.Utilities;

namespace BIM360FileTransfer.ViewModels
{
    internal class FileBrowseViewModel : BaseViewModel, IViewModel
    {
        private IList<CategoryViewModel> categoryTree;
        private IList<CategoryViewModel> selectedSourceCategoryTree;
        private IList<CategoryViewModel> selectedTargetCategoryTree;
        private IList<CategoryViewModel> targetCategoryTree;
        private Dictionary<CategoryViewModel, Stream> FileInfoStreamMap = new Dictionary<CategoryViewModel, Stream>();


        #region Constructor
        public FileBrowseViewModel()
        {
            selectedSourceCategoryTree = new ObservableCollection<CategoryViewModel>();
            selectedTargetCategoryTree = new ObservableCollection<CategoryViewModel>();
            FileBrowseCommand = new FileBrowseCommand(this);
            FileLoadCommand = new FileLoadCommand(this);
            FileTransferCommand = new FileTransferCommand(this);
        }
        #endregion

        #region Public Properties
        public IList<CategoryViewModel> CategoryTree
        {
            get { return categoryTree; }
            set
            {
                categoryTree = value;
                OnPropertyChanged("CategoryTree");
            }
        }

        public IList<CategoryViewModel> TargetCategoryTree
        {
            get { return targetCategoryTree; }
            set
            {
                targetCategoryTree = value;
                OnPropertyChanged("TargetCategoryTree");
            }
        }

        public IList<CategoryViewModel> SelectedSourceCategoryTree
        {
            get { return selectedSourceCategoryTree; }
            set
            {
                selectedSourceCategoryTree = value;
                OnPropertyChanged("SelectedSourceCategoryTree");
            }
        }

        public IList<CategoryViewModel> SelectedTargetCategoryTree
        {
            get { return selectedTargetCategoryTree; }
            set
            {
                selectedTargetCategoryTree = value;
                OnPropertyChanged("SelectedTargetCategoryTree");
            }
        }
        #endregion

        #region Get Category
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
                
                // Create a deep copy of the source tree to build the target tree.
                TargetCategoryTree = new List<CategoryViewModel>();
                foreach(var tree in CategoryTree)
                {
                    TargetCategoryTree.Add(TreeHelper.DeepClone<CategoryViewModel>(tree));
                }
            }
        }

        public void GetCategoryAsync()
        {
            var hubId = GetHub();
            CategoryTree = GetCategoryTree(hubId);
            TargetCategoryTree = new List<CategoryViewModel>();
            foreach (var tree in CategoryTree)
            {
                TargetCategoryTree.Add(TreeHelper.DeepClone<CategoryViewModel>(tree));
            }
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
            //var categoryTree = new ObservableCollection<CategoryViewModel> { GetProjects(hubId) };
            //return categoryTree;

            var categoryTree = new ObservableCollection<CategoryViewModel>();

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
                //GetChildrenCategory(hubId, thisCategory);
                categoryTree.Add(thisCategory);
            }
            return categoryTree;
        }

        //private CategoryViewModel GetProjects(string hubId)
        //{
        //    var root = new CategoryModel("Projects", "root");
        //    var rootCategory = new PublicCategoryCore(root);

        //    ProjectsApi projectsAPIInstance = new ProjectsApi();
        //    projectsAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;

        //    var response = projectsAPIInstance.GetHubProjects(hubId);

        //    foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
        //    {
        //        var type = objInfo.Value.type;
        //        var projectId = objInfo.Value.id;
        //        var rootFolderId = objInfo.Value.relationships.rootFolder.data.id;
        //        var name = objInfo.Value.attributes.name;

        //        var entity = new CategoryModel(rootFolderId, projectId, name, type);
        //        var thisCategory = new PublicCategoryCore(entity);
        //        //thisCategory.Parent = rootCategory;
        //        GetChildrenCategory(hubId, thisCategory);
        //        rootCategory.Children.Add(thisCategory);
        //    }

            
        //    return rootCategory;
        //}

        //private void GetChildrenCategory(string hubId, CategoryViewModel rootCategory)
        //{
            
        //    if (rootCategory.CategoryType == "projects")
        //    {
        //        var folderAPIInstance = new FoldersApi();
        //        folderAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
        //        var response = folderAPIInstance.GetFolderContents(rootCategory.CategoryProjectId, rootCategory.CategoryId);
        //        foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
        //        {
        //            var type = objInfo.Value.type;
        //            var folderId = objInfo.Value.id;
        //            var name = objInfo.Value.attributes.name;

        //            if (name == "Plans" || name == "Project Files")
        //            {
        //                var entity = new CategoryModel(folderId, rootCategory.CategoryProjectId, name, type);
        //                var thisCategory = new PublicCategoryCore(entity);
        //                //thisCategory.Parent = rootCategory;
        //                GetChildrenCategory(hubId, thisCategory);
        //                rootCategory.Children.Add(thisCategory);
        //            }
        //        }
        //    }
        //    else if (rootCategory.CategoryType == "folders")
        //    {
        //        if (rootCategory.CategoryName == "Plans" || rootCategory.CategoryName == "Revit Upgrade Report")
        //        {
        //            return;
        //        }
        //        var apiInstance = new FoldersApi();
        //        var response = apiInstance.GetFolderContents(rootCategory.CategoryProjectId, rootCategory.CategoryId);

        //        bool isItemExist = false;

        //        foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
        //        {
        //            var type = objInfo.Value.type;
        //            if (type == "items")
        //            {
        //                isItemExist = true;
        //                continue;
        //            }
        //            else 
        //            {
        //                var folderId = objInfo.Value.id;
        //                var name = objInfo.Value.attributes.name;

        //                var entity = new CategoryModel(folderId, rootCategory.CategoryProjectId, name, type);
        //                var thisCategory = new PublicCategoryCore(entity);
        //                //thisCategory.Parent = rootCategory;
        //                GetChildrenCategory(hubId, thisCategory);
        //                rootCategory.Children.Add(thisCategory);
        //            }
        //        }
        //        if (isItemExist)
        //        {
        //            foreach (KeyValuePair<string, dynamic> storageObjInfo in new DynamicDictionaryItems(response.included))
        //            {
        //                var new_type = storageObjInfo.Value.type;
        //                if (new_type == "versions")
        //                {
        //                    var id = storageObjInfo.Value.relationships.storage.data.id;
        //                    var storage_object_id = id.Substring(id.LastIndexOf('/') + 1);
        //                    var bucket_id = id.Substring(0, id.LastIndexOf('/')).Substring(id.Substring(0, id.LastIndexOf('/') + 1).LastIndexOf(':') + 1);
        //                    var name = storageObjInfo.Value.attributes.displayName + " v" + storageObjInfo.Value.attributes.versionNumber.ToString();

        //                    var entity = new CategoryModel(storage_object_id, bucket_id, rootCategory.CategoryProjectId, name, new_type);
        //                    var thisCategory = new PublicCategoryCore(entity);
        //                    thisCategory.IsVisible = false;
        //                    //thisCategory.Parent = rootCategory;
        //                    rootCategory.Children.Add(thisCategory);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Transfer File
        internal void TransferFile()
        {
            DownloadFile();
            UploadFile();
        }

        internal void DownloadFile()
        {
            foreach(var item in selectedSourceCategoryTree)
            {
                if (item.CategoryType != "versions") continue;
                var objectAPIInstance = new ObjectsApi();
                objectAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
                var bucketKey = item.CategoryBucketId;  // string | URL-encoded bucket key
                var objectName = item.CategoryId;  // string | URL-encoded object name

                try
                {
                    Stream result = objectAPIInstance.GetObject(bucketKey, objectName);
                    FileInfoStreamMap[item] = result;

                    //var filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "\\Resources";
                    //DirectoryInfo info = new DirectoryInfo(filePath);
                    //if (!info.Exists)
                    //{
                    //    info.Create();
                    //}
                    //string path = Path.Combine(filePath, item.CategoryName.Substring(0, item.CategoryName.LastIndexOf(' ')));
                    //using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                    //{
                    //    result.CopyTo(outputFileStream);
                    //}
                }
                catch (Exception e)
                {
                    throw new Exception("Exception when calling ObjectsApi.GetObject: " + e.Message);
                }
            }
        }

        private CreateStorage CreateStorageBody(string folderId, KeyValuePair<CategoryViewModel, Stream> fileInfoStreamMap)
        {
            var jsonapi = new JsonApiVersionJsonapi(new JsonApiVersionJsonapi.VersionEnum());
            var attributes = new CreateStorageDataAttributes(fileInfoStreamMap.Key.CategoryName.Substring(0, fileInfoStreamMap.Key.CategoryName.LastIndexOf(' ')), new BaseAttributesExtensionObject("", "", new JsonApiLink("")));
            var target = new CreateStorageDataRelationshipsTarget(new StorageRelationshipsTargetData(new StorageRelationshipsTargetData.TypeEnum(), folderId));
            var relationships = new CreateStorageDataRelationships(target);
            var data = new CreateStorageData(new CreateStorageData.TypeEnum(), attributes, relationships);
            var createStorageBody = new CreateStorage(jsonapi, data); // CreateStorage | describe the file the storage is created for
            return createStorageBody;
        }

        private CreateItem CreateItemBody(string folderId, string target_storage_object_id, KeyValuePair<CategoryViewModel, Stream> fileInfoStreamMap)
        {
            var jsonapi = new JsonApiVersionJsonapi(new JsonApiVersionJsonapi.VersionEnum());
            var createItemDataAttributes = new CreateItemDataAttributes(fileInfoStreamMap.Key.CategoryName.Substring(0, fileInfoStreamMap.Key.CategoryName.LastIndexOf(' ')), new BaseAttributesExtensionObject("items:autodesk.bim360:File", "1.0", new JsonApiLink("")));
            var tip = new CreateItemDataRelationshipsTip(new CreateItemDataRelationshipsTipData(new CreateItemDataRelationshipsTipData.TypeEnum(), new CreateItemDataRelationshipsTipData.IdEnum()));
            var target = new CreateStorageDataRelationshipsTarget(new StorageRelationshipsTargetData(new StorageRelationshipsTargetData.TypeEnum(), folderId));
            var createItemDataRelationships = new CreateItemDataRelationships(tip, target);
            var data = new CreateItemData(new CreateItemData.TypeEnum(), createItemDataAttributes, createItemDataRelationships);

            var createStorageDataAttributes = new CreateStorageDataAttributes(fileInfoStreamMap.Key.CategoryName.Substring(0, fileInfoStreamMap.Key.CategoryName.LastIndexOf(' ')), new BaseAttributesExtensionObject("versions:autodesk.bim360:File", "1.0", new JsonApiLink("")));
            var createStorageDataRelationships = new CreateItemRelationships(new CreateItemRelationshipsStorage(new CreateItemRelationshipsStorageData(new CreateItemRelationshipsStorageData.TypeEnum(), target_storage_object_id)));
            var included = new List<CreateItemIncluded>() { new CreateItemIncluded(new CreateItemIncluded.TypeEnum(), new CreateItemIncluded.IdEnum(), createStorageDataAttributes, createStorageDataRelationships) };

            var postItemBody = new CreateItem(jsonapi, data, included); // CreateItem | describe the item to be created
            return postItemBody;
        }

        internal void UploadFile()
        {
            foreach (var item in selectedTargetCategoryTree)
            {
                if (item.CategoryType == "projects") continue;

                var projectsAPIInstance = new ProjectsApi();
                var folderId = item.CategoryId;
                var projectId = item.CategoryProjectId;  // string | the `project id`
                
                try
                {
                    foreach (KeyValuePair<CategoryViewModel, Stream> fileInfoStreamMap in FileInfoStreamMap)
                    {
                        bool isExisted = false;
                        string itemId = "";
                        foreach (var child in item.Children)
                        {
                            if (child.CategoryType == "items" && fileInfoStreamMap.Key.CategoryName.Contains(child.CategoryName))
                            {
                                isExisted = true;
                                itemId = child.CategoryId;
                                break;
                            }
                        }
                        var createStorageBody = CreateStorageBody(folderId, fileInfoStreamMap); // CreateStorage | describe the file the storage is created for
                        var storageCreateResult = projectsAPIInstance.PostStorage(projectId, createStorageBody);
                        var target_storage_object_id = storageCreateResult.data.id;
                        var target_object_id = target_storage_object_id.Substring(target_storage_object_id.LastIndexOf('/') + 1);
                        var target_bucket_key = target_storage_object_id.Substring(0, target_storage_object_id.LastIndexOf('/')).Substring(target_storage_object_id.Substring(0, target_storage_object_id.LastIndexOf('/') + 1).LastIndexOf(':') + 1);

                        var objectsAPIInstance = new ObjectsApi();
                        objectsAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
                        var bucketKey = target_bucket_key;  // string | URL-encoded bucket key
                        var objectName = target_object_id;  // string | URL-encoded object name
                        var contentLength = Convert.ToInt32(fileInfoStreamMap.Value.Length);  // int? | Indicates the size of the request body.
                        var uploadFileBody = fileInfoStreamMap.Value;  // System.IO.Stream | 

                        try
                        {
                            var uploadFileResult = objectsAPIInstance.UploadObject(bucketKey, objectName, contentLength, uploadFileBody);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Exception when calling ObjectApi.UploadObject: " + e.Message);
                        }

                        if (!isExisted)
                        {
                            
                            var itemsAPIInstance = new ItemsApi();
                            itemsAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
                            var postItemBody = CreateItemBody(folderId, target_storage_object_id, fileInfoStreamMap);

                            using (StreamWriter file = File.CreateText(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "\\Resources\\postItemBody.json"))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                serializer.TypeNameHandling = TypeNameHandling.None;
                                serializer.Serialize(file, postItemBody);
                            }

                            try
                            {
                                var postItemResult = itemsAPIInstance.PostItem(projectId, postItemBody);
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Exception when calling ItemsApi.PostItem: " + e.Message);
                            }
                        }
                        else
                        {
                            var jsonapi = new JsonApiVersionJsonapi(new JsonApiVersionJsonapi.VersionEnum());
                            var createItemDataAttributes = new CreateStorageDataAttributes(fileInfoStreamMap.Key.CategoryName.Substring(0, fileInfoStreamMap.Key.CategoryName.LastIndexOf(' ')), new BaseAttributesExtensionObject("versions:autodesk.bim360:File", "1.0", new JsonApiLink("")));

                            var createVersionDataRelationshipsItem = new CreateVersionDataRelationshipsItem(new CreateVersionDataRelationshipsItemData(new CreateVersionDataRelationshipsItemData.TypeEnum(), itemId));
                            var createItemRelationshipsStorage = new CreateItemRelationshipsStorage(new CreateItemRelationshipsStorageData(new CreateItemRelationshipsStorageData.TypeEnum(), target_storage_object_id));
                            var createItemDataRelationships = new CreateVersionDataRelationships(createVersionDataRelationshipsItem, createItemRelationshipsStorage);


                            var createVersionData = new CreateVersionData(new CreateVersionData.TypeEnum(), createItemDataAttributes, createItemDataRelationships);

                            var createVersionBody = new CreateVersion(jsonapi, createVersionData); // CreateVersion | describe the version to be created

                            try
                            {
                                var result = projectsAPIInstance.PostVersion(projectId, createVersionBody);

                            }
                            catch (Exception e)
                            {
                                Debug.Print("Exception when calling ProjectsApi.PostVersion: " + e.Message);
                            }
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Exception when calling ProjectsApi.PostStorage: " + e.Message);
                }
            }
        }

        #endregion

        #region ICommand
        public bool CanFileBrowse
        {
            get
            {
                if (User.FORGE_CODE is null)
                {
                    return false;
                }
                return true;
            }
        }

        public bool CanFileTransfer
        {
            get
            {
                if (selectedTargetCategoryTree.Count == 0 || selectedSourceCategoryTree.Count == 0)
                {
                    return false;
                }
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

        public ICommand FileTransferCommand
        {
            get;
            private set;
        }
        #endregion
    }
}
