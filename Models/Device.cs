using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Models
{
    public class Device
    {
        public Device(int id_device, string name, string model, string description, /*object id_category, object id_sector,*/ Category category, Sector sector)
        {
            //ID_category = 0;
            //ID_sector = 0;

            //if (id_category != DBNull.Value)
            //{
            //    ID_category = Convert.ToInt32(id_category);
            //}
            //if (id_sector != DBNull.Value)
            //{
            //    ID_sector = Convert.ToInt32(id_sector);
            //}

            ID_device = id_device;
            Name = name;
            Model = model;
            Description = description;
            Category = category;
            Sector = sector;
        }

        [Key]
        public int ID_device { get; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        //public int ID_category { get; set; }

        //public int ID_sector { get; set; }

        public Category Category { get; set; }

        public Sector Sector { get; set; }
    }
}
