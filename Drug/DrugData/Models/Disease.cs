using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Disease
    {
        public int DiseaseId { get; set; }

        [Required]
        [StringLength(100)]
        public string DiseaseName { get; set; }

        [Required]
        [StringLength(3)]
        public string ICD { get; set; }

        public virtual ICollection<DrugDisease> DrugDiseases { get; set; }
    }
}
