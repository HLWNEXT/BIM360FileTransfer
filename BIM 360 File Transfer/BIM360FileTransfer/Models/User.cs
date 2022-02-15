using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.Models
{
    internal static class User
    {
        public const string FORGE_CLIENT_ID = "Oc1Dgsd4bxY5hbfvYOsuHCkZTyI1ef7q";
        public const string FORGE_CLIENT_SECRET = "PwA4iw3Ant6MlkQZ";
        public const string FORGE_CALLBACK_URL = "http://sampleapp.com/oauth/callback?foo=bar";
        public const string FORGE_BASE_URL = "https://developer.api.autodesk.com";
        public const string FORGE_SCOPE = "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete"; // Full scope access.
        public const string FORGE_GRANT_TYPE = "authorization_code";
        public const string Forge_Code_Pattern = "&code=";
        public static string FORGE_CODE { get; set; }
        public static dynamic FORGE_INTERNAL_TOKEN { get; set; }
    }
}
