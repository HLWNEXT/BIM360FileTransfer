using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.Models
{
    internal static class Settings
    {
        public static string JSON_PATH { get; set; } = "";
        public static bool CREATE_NEW_VERSION { get; set; } = true;
    }
}
