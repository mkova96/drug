using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class CategoryViewModel
    {
        [Required(ErrorMessage = "Naziv bolesti je obavezan.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Međunarodni klasifikacijski broj bolesti je obavezan.")]
        [StringLength(3)]
        public string MKB { get; set; }

    }
}
