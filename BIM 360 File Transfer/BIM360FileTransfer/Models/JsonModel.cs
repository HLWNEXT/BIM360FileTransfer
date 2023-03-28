using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.Models
{
    internal class JsonModel
    {
        public class SourceItem
        {
            public string model_name { get; set; }
            public string path { get; set; }

            public SourceItem(string _name, string _path)
            {
                model_name = _name;
                path = _path;
            }
        }

        public class TargetItem
        {
            public string folder_name { get; set; }
            public string path { get; set; }

            public TargetItem(string _name, string _path)
            {
                folder_name = _name;
                path = _path;
            }
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

        public Source source { get; set; }
        public Target target { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string selectedDate { get; set; }
        public string selectedTime { get; set; }

        public JsonModel()
        {
            source = new Source();
            target = new Target();
            startDate = string.Empty;
            endDate = string.Empty;
            selectedDate = string.Empty;
            selectedTime = string.Empty;
        }

    }
}
