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
using System.Windows;
using Microsoft.Win32;

namespace BIM360FileTransfer.ViewModels
{
    internal class FileBrowseViewModel : BaseViewModel, IViewModel
    {
        #region Data
        SettingViewModel settingViewModel;
        private IList<CategoryViewModel> categoryTree;
        private IList<CategoryViewModel> selectedSourceCategoryTree;
        private IList<CategoryViewModel> selectedTargetCategoryTree;
        private IList<CategoryViewModel> targetCategoryTree;
        private string filePaths;
        private Dictionary<CategoryViewModel, Stream> FileInfoStreamMap = new Dictionary<CategoryViewModel, Stream>();
        #endregion

        #region Constructor
        public FileBrowseViewModel()
        {
            selectedSourceCategoryTree = new ObservableCollection<CategoryViewModel>();
            selectedTargetCategoryTree = new ObservableCollection<CategoryViewModel>();
            FileBrowseCommand = new FileBrowseCommand(this);
            FileLoadCommand = new FileLoadCommand(this);
            LoadLocalFilesCommand = new LoadLocalFilesCommand(this);
            FileTransferCommand = new FileTransferCommand(this);
            FileTransferExecuteCommand = new FileTransferExecuteCommand(this);
            FileTransferAbortCommand = new FileTransferAbortCommand(this);
            SaveJsonCommand = new SaveJsonCommand(this);

            settingViewModel = new SettingViewModel(this);
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

        public string FilePaths
        {
            get { return filePaths; }
            set
            {
                filePaths = value;
                OnPropertyChanged("FilePaths");
            }
        }
        #endregion


        #region Get Category
        internal void GetCategoryLocal()
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".json"; // Default file extension
            dialog.Filter = "Text documents (.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                using (StreamReader file = File.OpenText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.TypeNameHandling = TypeNameHandling.All;
                    serializer.Formatting = Formatting.Indented;
                    CategoryTree = (IList<CategoryViewModel>)serializer.Deserialize(file, typeof(IList<CategoryViewModel>));

                    // Create a deep copy of the source tree to build the target tree.
                    TargetCategoryTree = new List<CategoryViewModel>();
                    foreach (var tree in CategoryTree)
                    {
                        TargetCategoryTree.Add(TreeHelper.DeepClone<CategoryViewModel>(tree));
                    }
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
                TargetCategoryTree.Add(TreeHelper.DeepClone(tree));
            }
            //SaveCategoryToJson(CategoryTree);
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
                thisCategory.CategoryPath = name;
                //thisCategory.Parent = rootCategory;
                //GetChildrenCategory(hubId, thisCategory);
                categoryTree.Add(thisCategory);
            }
            return categoryTree;
        }
        #endregion

        #region Transfer File
        internal void TransferFile()
        {
            
            settingViewModel.OpenSettingWindow();

            //string messageBoxText = "Do you want to work on these tasks? \n Transfer: a to b \n Transfer: c to d";
            //string caption = "File Transfer Processor";
            //MessageBoxButton button = MessageBoxButton.YesNoCancel;
            //MessageBoxImage icon = MessageBoxImage.Warning;
            //MessageBoxResult result;

            //result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            //if (result == MessageBoxResult.No) return;


        }

        internal void AbortTransfer()
        {
            settingViewModel.SettingWindow.Close();
        }

        internal void ExecuteTransfer()
        {
            DownloadFile();
            UploadFileAsync();
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

                    // Save to local
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

        internal void LoadLocalFile()
        {
            FilePaths = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Autodesk Revit Project (*.rvt)|*.rvt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    FilePaths += (filePath + Environment.NewLine);
                    var filename = new FileInfo(filePath);

                    MemoryStream localSourceStream = new MemoryStream();
                    using (FileStream localSource = File.Open(@filePath, FileMode.Open))
                    {

                        Console.WriteLine("Source length: {0}", localSource.Length.ToString());

                        // Copy source to destination.
                        localSource.CopyTo(localSourceStream);
                        localSourceStream.Position = 0;
                        var id = Guid.NewGuid();
                        var entity = new CategoryModel(id.ToString(), "", filename.Name + " v1", "items");
                        var thisCategory = new PublicCategoryCore(entity);
                        FileInfoStreamMap[thisCategory] = localSourceStream;
                    }
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

        internal async Task UploadFileAsync()
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
                        fileInfoStreamMap.Value.Position = 0;
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

                            //using (StreamWriter file = File.CreateText(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "\\Resources\\postItemBody.json"))
                            //{
                            //    JsonSerializer serializer = new JsonSerializer();
                            //    serializer.TypeNameHandling = TypeNameHandling.None;
                            //    serializer.Serialize(file, postItemBody);
                            //}

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
                            //string messageBoxText = "The following file is already existed in the target folder. Do you want to create a new version? \n projects/folderPath/fileName";
                            //string caption = "File Transfer Processor";
                            //MessageBoxButton button = MessageBoxButton.YesNoCancel;
                            //MessageBoxImage icon = MessageBoxImage.Warning;
                            //MessageBoxResult result;
                            MessageBoxModel postVersionMessageBox = new MessageBoxModel("The following file is already existed in the target folder. Do you want to create a new version? \n projects/folderPath/fileName",
                                                                                        "File Transfer Processor",
                                                                                        MessageBoxButton.YesNoCancel,
                                                                                        MessageBoxImage.Warning);
                            postVersionMessageBox.result = MessageBox.Show(postVersionMessageBox.messageBoxText, postVersionMessageBox.caption, postVersionMessageBox.button, postVersionMessageBox.icon, MessageBoxResult.Yes);
                            if (postVersionMessageBox.result == MessageBoxResult.No) return;


                            var jsonapi = new JsonApiVersionJsonapi(new JsonApiVersionJsonapi.VersionEnum());
                            var createItemDataAttributes = new CreateStorageDataAttributes(fileInfoStreamMap.Key.CategoryName.Substring(0, fileInfoStreamMap.Key.CategoryName.LastIndexOf(' ')), new BaseAttributesExtensionObject("versions:autodesk.bim360:File", "1.0", new JsonApiLink("")));

                            var createVersionDataRelationshipsItem = new CreateVersionDataRelationshipsItem(new CreateVersionDataRelationshipsItemData(new CreateVersionDataRelationshipsItemData.TypeEnum(), itemId));
                            var createItemRelationshipsStorage = new CreateItemRelationshipsStorage(new CreateItemRelationshipsStorageData(new CreateItemRelationshipsStorageData.TypeEnum(), target_storage_object_id));
                            var createItemDataRelationships = new CreateVersionDataRelationships(createVersionDataRelationshipsItem, createItemRelationshipsStorage);


                            var createVersionData = new CreateVersionData(new CreateVersionData.TypeEnum(), createItemDataAttributes, createItemDataRelationships);

                            var createVersionBody = new CreateVersion(jsonapi, createVersionData); // CreateVersion | describe the version to be created

                            try
                            {
                                var postVersionResult = projectsAPIInstance.PostVersion(projectId, createVersionBody);

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

                //item.Children.Clear();
                //await Task.Run(() => item.GetChildrenCategoryAsync(item));
            }
        }
        #endregion


        #region Reoccurring Task
        public void SaveCategoryToJson()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Reoccurring_Task"; // Default file name
            dialog.DefaultExt = ".json"; // Default file extension
            dialog.Filter = "Text documents (.json)|*.json"; // Filter files by extension

            // Show save file dialog box
            bool? result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dialog.FileName;
                using (StreamWriter file = File.CreateText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.TypeNameHandling = TypeNameHandling.All;
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Serialize(file, categoryTree);
                }
            }

            //// Create a message box. Let use choose if they want to save json.
            //MessageBoxModel saveJsonMessageBox = new MessageBoxModel("File transfered sucessfully. Do you want to save the log to JSON?",
            //                                                         "File Transfer Processor",
            //                                                         MessageBoxButton.YesNoCancel,
            //                                                         MessageBoxImage.Warning);
            //saveJsonMessageBox.result = MessageBox.Show(saveJsonMessageBox.messageBoxText, saveJsonMessageBox.caption, saveJsonMessageBox.button, saveJsonMessageBox.icon, MessageBoxResult.Yes);

            //if (saveJsonMessageBox.result == MessageBoxResult.Yes)
            //{
            //    // Configure save file dialog box
            //    var dialog = new Microsoft.Win32.SaveFileDialog();
            //    dialog.FileName = "Document"; // Default file name
            //    dialog.DefaultExt = ".json"; // Default file extension
            //    dialog.Filter = "Text documents (.json)|*.json"; // Filter files by extension

            //    // Show save file dialog box
            //    bool? result = dialog.ShowDialog();

            //    // Process save file dialog box results
            //    if (result == true)
            //    {
            //        // Save document
            //        string filename = dialog.FileName;
            //        using (StreamWriter file = File.CreateText(filename))
            //        {
            //            JsonSerializer serializer = new JsonSerializer();
            //            serializer.TypeNameHandling = TypeNameHandling.All;
            //            serializer.NullValueHandling = NullValueHandling.Ignore;
            //            serializer.Serialize(file, categoryTree);
            //        }
            //    }
            //}
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

        public bool CanFileLoad
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
                if (selectedTargetCategoryTree.Count == 0 || FileInfoStreamMap.Keys.Count == 0)
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

        public ICommand LoadLocalFilesCommand
        {
            get;
            private set;
        }

        public ICommand FileTransferCommand
        {
            get;
            private set;
        }

        public ICommand FileTransferExecuteCommand
        {
            get;
            private set;
        }

        public ICommand FileTransferAbortCommand
        {
            get;
            private set;
        }

        public ICommand SaveJsonCommand
        {
            get;
            private set;
        }

        public ICommand SettingCommand
        {
            get;
            private set;
        }
        #endregion
    }
}
