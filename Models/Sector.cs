using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Sector()
        {
            ID_sector = 0;
            Name = "";
            Description = "";
        }

        [Key]
        public int ID_sector { get; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
