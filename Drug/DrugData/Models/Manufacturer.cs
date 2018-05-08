using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LijekData.Models
{
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }

        [Required]
        [StringLength(100)]
        public string ManufacturerName { get; set; }

        [DataType(DataType.MultilineText)]
        public String About { get; set; }


        public virtual ICollection<Drug> Drugs { get; set; }

    }
}
