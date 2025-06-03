using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Entities
{
    public class Category
    {
        public Guid CategoryId { get; set; }    //PK
        public string Name { get; set; }
        public string Description { get; set; }

        //Nagivation properties
        public ICollection<TaskCategory> TaskCategories { get; set; }
    }
}
