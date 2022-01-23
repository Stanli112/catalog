using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Models
{
    public class Sector
    {
        public Sector(int id_sector, string name, string description)
        {
            ID_sector = id_sector;
            Name = name;
            Description = description;
        }

        public int ID_sector { get; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
