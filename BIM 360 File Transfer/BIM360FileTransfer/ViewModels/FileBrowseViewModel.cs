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
        public async Task<IList<TreeNode>> GetFileAsync(string id)
        {
            IList<TreeNode> nodes = new List<TreeNode>();

            if (id == "#") // root
            {
                // Get all buckets
                BucketsApi appBckets = new BucketsApi();
                appBckets.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;

                // to simplify, let's return only the first 100 buckets
                dynamic buckets = await appBckets.GetBucketsAsync("US", 100);
                foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(buckets.items))
                {
                    nodes.Add(new TreeNode(bucket.Value.bucketKey, bucket.Value.bucketKey.Replace(User.FORGE_CLIENT_ID + "-", string.Empty), "bucket", true));
                }
            }
            else
            {
                // as we have the id (bucketKey), let's return all 
                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
                var objectsList = await objects.GetObjectsAsync(id, 100);
                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
                {
                    nodes.Add(new TreeNode(Base64Encode((string)objInfo.Value.objectId),
                      objInfo.Value.objectKey, "object", false));
                }
            }
            return nodes;
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
