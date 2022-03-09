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

        public JsonModel()
        {
            source = new Source();
            target = new Target();
        }


        public class Source
        {
            public List<SourceItem> sourceItems { get; set; }

            public Source()
            {
                sourceItems = new List<SourceItem>();
            }
        }

        public class Target
        {
            public List<TargetItem> targetItems { get; set; }

            public Target()
            {
                targetItems = new List<TargetItem>();
            }
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
