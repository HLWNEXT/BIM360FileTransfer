using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.Interfaces
{
    public interface ICategory
    {
        List<ICategory> Children { get; set; }

        //string ParentId { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        string Id { get; set; }
        string ProjectId { get; set; }
        int Level { get; set; }

        List<string> Subjects { get; set; }


    }
}
