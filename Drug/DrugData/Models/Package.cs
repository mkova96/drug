using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Package
    {
        public int PackageId { get; set; }

        [Required]
        [StringLength(100)]
        public string PackageType { get; set; }
        public virtual ICollection<Medication> Drugs { get; set; }
    }
}
