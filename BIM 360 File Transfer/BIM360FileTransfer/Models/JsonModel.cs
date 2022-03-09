using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.Models
{
    internal class JsonModel
    {
        public Source source { get; set; }
        public Target target { get; set; }
        public class Source
        {
            public List<SourceItem> sourceItems { get; set; }
        }

        public class Target
        {
            public List<SourceItem> targetItems { get; set; }
        }

        public class SourceItem
        {
            public string model_name { get; set; }
            public string path { get; set; }
        }

        public class TargetItem
        {
            public string folder_name { get; set; }
            public string path { get; set; }
        }

    }
}
