using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.Models
{
    public class CategoryModel : ICategory, IModel
    {
        private List<ICategory> children;
        public List<ICategory> Children
        {
            get
            {
                List<ICategory> results;
                if ((results = children) == null)
                    results = children = new List<ICategory>();
                return results;
            }
            set { children = value; }
        }
        public string ParentId { get; set; }
        public List<string> Subjects { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }

        public string Id { get; }

        public string Name { get; set; }

        public CategoryModel(string id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
            Subjects = new List<string>();
        }
    }
}
