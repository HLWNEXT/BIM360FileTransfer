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

namespace BIM360FileTransfer.ViewModels
{
    internal class FileBrowseViewModel
    {
        private const string FORGE_CLIENT_ID = "Oc1Dgsd4bxY5hbfvYOsuHCkZTyI1ef7q";
        private const string FORGE_CLIENT_SECRET = "PwA4iw3Ant6MlkQZ";
        private const string FORGE_CALLBACK_URL = "http://sampleapp.com/oauth/callback?foo=bar";
        private const string FORGE_BASE_URL = "https://developer.api.autodesk.com";
        private const string FORGE_SCOPE = "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete"; // Full scope access.
        private const string FORGE_GRANT_TYPE = "authorization_code";
        private const string Forge_Code_Pattern = "&code=";
        private static string FORGE_CODE { get; set; }
        private static dynamic FORGE_INTERNAL_TOKEN { get; set; }


        public ChromiumWebBrowser authBrowser;
        public OAuthWindow oAuthWindow;


        /// <summary>
        /// Get a list of buckets (id=#) or list of objects (id=bucketKey)
        /// </summary>
        public async Task<IList<TreeNode>> GetOSSAsync(string id)
        {
            IList<TreeNode> nodes = new List<TreeNode>();

            if (id == "#") // root
            {
                // Get all buckets
                BucketsApi appBckets = new BucketsApi();
                appBckets.Configuration.AccessToken = FORGE_INTERNAL_TOKEN.access_token;

                // to simplify, let's return only the first 100 buckets
                dynamic buckets = await appBckets.GetBucketsAsync("US", 100);
                foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(buckets.items))
                {
                    nodes.Add(new TreeNode(bucket.Value.bucketKey, bucket.Value.bucketKey.Replace(FORGE_CLIENT_ID + "-", string.Empty), "bucket", true));
                }
            }
            else
            {
                // as we have the id (bucketKey), let's return all 
                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = FORGE_INTERNAL_TOKEN.access_token;
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
