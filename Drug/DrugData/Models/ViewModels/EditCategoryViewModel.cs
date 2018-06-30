using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditCategoryViewModel
    {
        public int DiseaseId { get; set; }

        [Required(ErrorMessage = "Naziv bolesti je obavezan.")]
        public string DiseaseName { get; set; }

        [Required(ErrorMessage = "Međunarodni klasifikacijski broj bolesti je obavezan.")]
        [StringLength(3)]
        public string ICD { get; set; }

    }
}
