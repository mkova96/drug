using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Disease
    {
        public int DiseaseId { get; set; }

        [Required(ErrorMessage = "Naziv bolesti je obavezan.")]
        public string DiseaseName { get; set; }

        [Required(ErrorMessage = "Međunarodni klasifikacijski broj bolesti je obavezan.")]
        [StringLength(3)]
        public string ICD { get; set; }

        public virtual ICollection<DrugDisease> DrugDiseases { get; set; }
    }
}
