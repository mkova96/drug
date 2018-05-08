using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Country
    {
        public int CountryId { get; set; }

        [Required]
        [StringLength(100)]
        public String CountryName { get; set; }

        public virtual ICollection<City> Cities { get; set; }

    }
}
