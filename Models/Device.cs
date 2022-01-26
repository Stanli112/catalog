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
        public Device()
        {
            ID_device = 0;
            Name = "";
            Model = "";
            Description = "";
            Category = null;
            Sector = null;
            Image = new byte[0];
        }
        public Device(int id_device, string name, string model, string description, Category category, Sector sector, byte[] img = null)
        {
            ID_device = id_device;
            Name = name;
            Model = model;
            Description = description;
            Category = category;
            Sector = sector;
            Image = img;
        }

        [Key]
        public int ID_device { get; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public Category Category { get; set; }

        public Sector Sector { get; set; }

        public byte[] Image { get; set; }
    }
}
