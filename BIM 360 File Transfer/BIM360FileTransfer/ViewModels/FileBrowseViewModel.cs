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

        public FileBrowseViewModel()
        {
            FileBrowseCommand = new FileBrowseCommand(this);
        }

        /// <summary>
        /// Get a list of buckets (id=#) or list of objects (id=bucketKey)
        /// </summary>
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
            //hubsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6IlBub2xwQUF4REpUNzc5RFpicjJCYWpOdlhvaUFGWHZvM3E1c2Rub2Y0SmxPSjd4bmd3dTdiSW5ONTA1ZWRwdlQiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NzY5ODR9.RqWihcexXxT38dXwJ8qtdxGmPG96B4rNkhpvjvByU-DPylS215XLwy4fcJAwLS8uKX5ZH3JKKtjcr3oyZBUid3Mt9RItMN80j31prJHKFwkvyCyDbHMS0czhmzUR2VA_8rR2UWJHem-AUV4qRZ_-_jYsQ-QrxDFcB89iy9o_8zhdX_cP7Ui7PpT3cBhYVzMDD3ySiMUZYePN71rA10FwpetvnmZkPWN62RWHUSoMbGCbTn8bogEJa0MwnbzxY1Yp4YPZhfZET71pGoiMikyFTJlIOky0WV_jQyj78LFC1vSu43zILawoKtH-PGduQ-sghz3ys4qY-bvs4pIjq1W7JQ";

            var response = hubsAPIInstance.GetHubs();
            return response.data[0].id;
        }

        /// <summary>
        /// Get all projects.
        /// </summary>
        //public IList<CategoryViewModel> GetProjects(string hubId)
        //{
        //    var projects = new List<CategoryViewModel>();
        //    ProjectsApi projectsAPIInstance = new ProjectsApi();
        //    //hubs.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
        //    projectsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6IlBub2xwQUF4REpUNzc5RFpicjJCYWpOdlhvaUFGWHZvM3E1c2Rub2Y0SmxPSjd4bmd3dTdiSW5ONTA1ZWRwdlQiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NzY5ODR9.RqWihcexXxT38dXwJ8qtdxGmPG96B4rNkhpvjvByU-DPylS215XLwy4fcJAwLS8uKX5ZH3JKKtjcr3oyZBUid3Mt9RItMN80j31prJHKFwkvyCyDbHMS0czhmzUR2VA_8rR2UWJHem-AUV4qRZ_-_jYsQ-QrxDFcB89iy9o_8zhdX_cP7Ui7PpT3cBhYVzMDD3ySiMUZYePN71rA10FwpetvnmZkPWN62RWHUSoMbGCbTn8bogEJa0MwnbzxY1Yp4YPZhfZET71pGoiMikyFTJlIOky0WV_jQyj78LFC1vSu43zILawoKtH-PGduQ-sghz3ys4qY-bvs4pIjq1W7JQ";

        //    var response = projectsAPIInstance.GetHubProjects(hubId);

        //    foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
        //    {
        //        //projects.Add(new CategoryViewModel(Base64Encode((string)objInfo.Value.objectId),objInfo.Value.objectKey, "project", false));
        //    }
            
        //    return response.data[0].id;
        //}

        private IList<CategoryViewModel> GetCategoryTree(string hubId)
        {
            var categoryTree = new List<CategoryViewModel> { GetProjects(hubId) };
            return categoryTree;
        }

        private CategoryViewModel GetProjects(string hubId)
        {
            var root = new CategoryModel("Projects", "root");
            var rootCategory = new PublicCategoryCore(root);

            ProjectsApi projectsAPIInstance = new ProjectsApi();
            projectsAPIInstance.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            //projectsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6IlBub2xwQUF4REpUNzc5RFpicjJCYWpOdlhvaUFGWHZvM3E1c2Rub2Y0SmxPSjd4bmd3dTdiSW5ONTA1ZWRwdlQiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NzY5ODR9.RqWihcexXxT38dXwJ8qtdxGmPG96B4rNkhpvjvByU-DPylS215XLwy4fcJAwLS8uKX5ZH3JKKtjcr3oyZBUid3Mt9RItMN80j31prJHKFwkvyCyDbHMS0czhmzUR2VA_8rR2UWJHem-AUV4qRZ_-_jYsQ-QrxDFcB89iy9o_8zhdX_cP7Ui7PpT3cBhYVzMDD3ySiMUZYePN71rA10FwpetvnmZkPWN62RWHUSoMbGCbTn8bogEJa0MwnbzxY1Yp4YPZhfZET71pGoiMikyFTJlIOky0WV_jQyj78LFC1vSu43zILawoKtH-PGduQ-sghz3ys4qY-bvs4pIjq1W7JQ";

            var response = projectsAPIInstance.GetHubProjects(hubId);
            rootCategory.Children = new List<CategoryViewModel>();

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
                rootCategory.Children = new List<CategoryViewModel>();
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
                rootCategory.Children = new List<CategoryViewModel>();

                


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



        /// <summary>
        /// Model data for jsTree used on GetOSSAsync
        /// </summary>
        public class TreeNode
        {
            public TreeNode(string id, string text, string type, bool children)
            {
                this.id = id;
                this.text = text;
                this.type = type;
                this.children = children;
            }

            public string id { get; set; }
            public string text { get; set; }
            public string type { get; set; }
            public bool children { get; set; }
        }

        /// <summary>
        /// Base64 enconde a string
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string encodedText)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
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

    }
}
