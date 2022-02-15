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
                var hubId = GetHub();
                var projects = GetProjects(hubId);
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
        /// Get all projects.
        /// </summary>
        public string GetProjects(string hubId)
        {
            ProjectsApi projectsAPIInstance = new ProjectsApi();
            //hubs.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            projectsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6Im1yVDJGYm5jWnhjRjg0aHpKakZtSmQ2YTlZcEFmeEM1d0tkaGRxQUdLMmNJZ2ROTm9MYXQzZENEcXJtWUMyVUoiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NTMwNDR9.T9OoIaEDPTDl5xPl4RI3BwBVeOcQzeP3mPdILn1DdJyLDiDssUQHbiE4eiMD2zEKwEVwMoHCjj1dsjiB-o1Tu5FbHzD6HjaFlTAYRq9UlX6IByJiJJZsuUebXc7W9W4vL3933yKjG3S62T0_m5aFO2819WZqYBbfs9U-wJO8fwF60EjVxeCig9fos07sP_hR8fLT-fKhwHUKXMvgRFhXAwYdafO3kdCv2mhzmI0srakFhckhXeG2MTh7mEG8hJlOBSPVH8zR0Gua5wQgkF9LqXam15ox7PqyUoUxx5VfF5HtfpjdBhR7fHiAUfxH0CrCLMHAwissHJOn1a4C04Jqfw";

            var response = projectsAPIInstance.GetHubProjects(hubId);

            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(response.data))
            {
                //nodes.Add(new TreeNode(Base64Encode((string)objInfo.Value.objectId),
                //  objInfo.Value.objectKey, "object", false));
            }
            return response.data[0].id;
        }

        /// <summary>
        /// Get the working hub.
        /// </summary>
        public string GetHub()
        {
            HubsApi hubsAPIInstance = new HubsApi();
            //hubs.Configuration.AccessToken = User.FORGE_INTERNAL_TOKEN.access_token;
            hubsAPIInstance.Configuration.AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlU3c0dGRldUTzlBekNhSzBqZURRM2dQZXBURVdWN2VhIn0.eyJzY29wZSI6WyJkYXRhOndyaXRlIiwiZGF0YTpjcmVhdGUiLCJkYXRhOnNlYXJjaCIsImRhdGE6cmVhZCIsImJ1Y2tldDpyZWFkIiwiYnVja2V0OnVwZGF0ZSIsImJ1Y2tldDpjcmVhdGUiLCJidWNrZXQ6ZGVsZXRlIl0sImNsaWVudF9pZCI6Ik9jMURnc2Q0YnhZNWhiZnZZT3N1SENrWlR5STFlZjdxIiwiYXVkIjoiaHR0cHM6Ly9hdXRvZGVzay5jb20vYXVkL2Fqd3RleHA2MCIsImp0aSI6Im1yVDJGYm5jWnhjRjg0aHpKakZtSmQ2YTlZcEFmeEM1d0tkaGRxQUdLMmNJZ2ROTm9MYXQzZENEcXJtWUMyVUoiLCJ1c2VyaWQiOiJVNFVSS1AzUU5CTVEiLCJleHAiOjE2NDQ5NTMwNDR9.T9OoIaEDPTDl5xPl4RI3BwBVeOcQzeP3mPdILn1DdJyLDiDssUQHbiE4eiMD2zEKwEVwMoHCjj1dsjiB-o1Tu5FbHzD6HjaFlTAYRq9UlX6IByJiJJZsuUebXc7W9W4vL3933yKjG3S62T0_m5aFO2819WZqYBbfs9U-wJO8fwF60EjVxeCig9fos07sP_hR8fLT-fKhwHUKXMvgRFhXAwYdafO3kdCv2mhzmI0srakFhckhXeG2MTh7mEG8hJlOBSPVH8zR0Gua5wQgkF9LqXam15ox7PqyUoUxx5VfF5HtfpjdBhR7fHiAUfxH0CrCLMHAwissHJOn1a4C04Jqfw";


            var response = hubsAPIInstance.GetHubs();
            return response.data[0].id;
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
