using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using CefSharp.Wpf;
using CefSharp;
using Autodesk.Forge;
using BIM360FileTransfer.VIews;
using System.Collections.Generic;
using Autodesk.Forge.Model;
using BIM360FileTransfer.Models;

namespace BIM360FileTransfer.ViewModels
{
    internal class FileBrowseViewModel
    {
        /// <summary>
        /// Get a list of buckets (id=#) or list of objects (id=bucketKey)
        /// </summary>
        public IList<CategoryViewModel> GetCategoryAsync()
        {
            
            var hubId = GetHub();
            //var projects = GetProjects(hubId);
            var files = GetCategoryTree(hubId);

            return files;
        }

        /// <summary>
        /// Get the working hub.
        /// </summary>
        public string GetHub()
        {
            HubsApi hubsAPIInstance = new HubsApi();
            //hubs.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            hubsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6IlBub2xwQUF4REpUNzc5RFpicjJCYWpOdlhvaUFGWHZvM3E1c2Rub2Y0SmxPSjd4bmd3dTdiSW5ONTA1ZWRwdlQiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NzY5ODR9.RqWihcexXxT38dXwJ8qtdxGmPG96B4rNkhpvjvByU-DPylS215XLwy4fcJAwLS8uKX5ZH3JKKtjcr3oyZBUid3Mt9RItMN80j31prJHKFwkvyCyDbHMS0czhmzUR2VA_8rR2UWJHem-AUV4qRZ_-_jYsQ-QrxDFcB89iy9o_8zhdX_cP7Ui7PpT3cBhYVzMDD3ySiMUZYePN71rA10FwpetvnmZkPWN62RWHUSoMbGCbTn8bogEJa0MwnbzxY1Yp4YPZhfZET71pGoiMikyFTJlIOky0WV_jQyj78LFC1vSu43zILawoKtH-PGduQ-sghz3ys4qY-bvs4pIjq1W7JQ";

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
            var root = new CategoryModel("", "All Componentes", "root");
            var rootCategroy = new PublicCategoryCore(root);

            var projects = new List<CategoryViewModel>();
            ProjectsApi projectsAPIInstance = new ProjectsApi();
            //hubs.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            projectsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6IlBub2xwQUF4REpUNzc5RFpicjJCYWpOdlhvaUFGWHZvM3E1c2Rub2Y0SmxPSjd4bmd3dTdiSW5ONTA1ZWRwdlQiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NzY5ODR9.RqWihcexXxT38dXwJ8qtdxGmPG96B4rNkhpvjvByU-DPylS215XLwy4fcJAwLS8uKX5ZH3JKKtjcr3oyZBUid3Mt9RItMN80j31prJHKFwkvyCyDbHMS0czhmzUR2VA_8rR2UWJHem-AUV4qRZ_-_jYsQ-QrxDFcB89iy9o_8zhdX_cP7Ui7PpT3cBhYVzMDD3ySiMUZYePN71rA10FwpetvnmZkPWN62RWHUSoMbGCbTn8bogEJa0MwnbzxY1Yp4YPZhfZET71pGoiMikyFTJlIOky0WV_jQyj78LFC1vSu43zILawoKtH-PGduQ-sghz3ys4qY-bvs4pIjq1W7JQ";

            var response = projectsAPIInstance.GetHubProjects(hubId);

            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
            {
                var type = objInfo.Value.type;
                var id = objInfo.Value.id;
                var name = objInfo.Value.attributes.name;
                //projects.Add(new CategoryViewModel(Base64Encode((string)objInfo.Value.objectId),objInfo.Value.objectKey, "project", false));
                var entity = new CategoryModel(id, name, type);
                var thisCategory = new PublicCategoryCore(entity);
                thisCategory.Parent = rootCategroy;
                GetChildrenCategory(hubId, id, thisCategory);
                projects.Add(thisCategory);
                rootCategroy.Children.Add(thisCategory);
            }

            
            return rootCategroy;
        }

        private void GetChildrenCategory(string hubId, string projectId, CategoryViewModel rootCategroy)
        {
            //var directory = new DirectoryInfo(path);
            //var subDirectories = directory.GetDirectories();
            //if (subDirectories.Length == 0)
            //{
            //    var familyItems = new List<FamilyContentViewModel>();
            //    var files = directory.GetFiles();
            //    foreach (var item in files)
            //    {
            //        if (item.Extension == ".rfa")
            //        {
            //            var family = new FamilyContent();
            //            family.Name = item.Name.Substring(0, item.Name.Length - 4);
            //            family.ContentLocalPath = item.FullName;
            //            var entity = new FamilyContentViewModel(family);
            //            entity.PlaceFamily += PlaceFamilyReal;
            //            entity.LoadFamily += LoadFamilyReal;
            //            familyItems.Add(entity);
            //            var name = entity.Name;
            //        }
            //    }
            //    if (familyItems.Count != 0)
            //    {
            //        familyItems.ForEach(x => items.Add(x));

            //        rootCategroy.Model.Subjects = familyItems.Select(x => x.DisplayName).ToList();
            //    }

            //}
            //else
            //{
            //    foreach (var item in subDirectories)
            //    {
            //        var name = item.Name.Split('-').Last();
            //        var entity = new CategoryModel(name);
            //        var thisCategory = new PublicCategoryCore(entity);
            //        thisCategory.Parent = rootCategroy;
            //        GetChildrenCategory(item.FullName, thisCategory);
            //        rootCategroy.Children.Add(thisCategory);
            //    }
            //    rootCategroy.Children.ForEach(x => rootCategroy.Model.Subjects.AddRange(x.Model.Subjects));
            //}

            var entity = new CategoryModel("", "Project1", "project");
            var thisCategory = new PublicCategoryCore(entity);
            thisCategory.Parent = rootCategroy;
            var a = rootCategroy.Children;
            rootCategroy.Children.Add(thisCategory);
            //rootCategroy.Children.ForEach(x => rootCategroy.Model.Subjects.AddRange(x.Model.Subjects));
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

    }
}
