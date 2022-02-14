using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM_360_File_Transfer.ResponseModel
{
    public class DataManagementResponse
    {
        public JsonAPI jsonapi { get; set; }
        public Links links { get; set; }
        public List<Data> data { get; set; }
    }

    public class JsonAPI
    {
        public string version { get; set; }
    }

    public class Links
    {
        public class Self
        {
            public string href { get; set; }
        }

        public Self self { get; set; }
    }

    public class Data
    {
        public class Attributes
        {
            public class Extension
            {
                public class Schema
                {
                    public string href { get; set; }
                }

                public string type { get; set; }
                public string version { get; set; }
                public Schema schema { get; set; }
                public Data data { get; set; }

            }

            public string name { get; set; }
            public Extension extension { get; set; }
            public string region { get; set; }
            
        }


        public class Relationships
        {
            public class Projects
            {
                public class Links
                {
                    public class Related
                    {
                        public string href { get; set; }
                    }

                    public Related related { get; set; }
                }
                public Links links { get; set; }
            }

            public class PimCollection{
                public class Data
                {
                    public string id { get; set; }
                    public string type { get; set; }
                }

                public Data data { get; set; }
            }

            public Projects projects { get; set; }
            public PimCollection pimCollection { get; set; }

        }

        public string type { get; set; }
        public string id { get; set; } 
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
        public Links links { get; set; }
    }
}
