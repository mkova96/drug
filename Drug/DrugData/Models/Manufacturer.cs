using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public String About { get; set; }
        public string ImagePath { get; set; }


        public virtual ICollection<Medication> Drugs { get; set; }

    }
}
