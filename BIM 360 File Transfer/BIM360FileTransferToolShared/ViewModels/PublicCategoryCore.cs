using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    [Serializable]
    public class PublicCategoryCoreDeprecated : CategoryViewModel
    {
        
        public PublicCategoryCoreDeprecated(ICategory category) : base(category)
        {
            Model = category;
        }
    }
}
