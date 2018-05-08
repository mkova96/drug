using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class City
    {
        public int CityId { get; set; }
        public int PostCode { get; set; }

        [Required]
        [StringLength(100)]
        public String CityName { get; set; }
        public virtual Country Country { get; set; }

    }
}
