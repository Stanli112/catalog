using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Models
{
    public class Category
    {
        public Category(int id_category, string name, string description)
        {
            ID_category = id_category;
            Name = name;
            Description = description;
        }
        public int ID_category { get; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
