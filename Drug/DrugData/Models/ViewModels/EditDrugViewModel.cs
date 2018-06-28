using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditDrugViewModel
    {
        [Required(ErrorMessage = "Bolesti na koje je proizvod primjenjiv su obavezne")]
        [MinLength(1)]
        public IEnumerable<int> CategoryIds { get; set; }

        [Required(ErrorMessage = "Moguće nuspojave proizvoda su obavezne")]
        [MinLength(1)]
        public IEnumerable<int> SideEffectIds { get; set; }

        public Medication Drug { get; set; }

        public int PackageId { get; set; }
        public int CurrencyId { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public bool z { get; set; }
        public List<int> DrugIds { get; set; }

        public string SubstitutionType { get; set; }

        public int ManufacturerId { get; set; }

        public EditDrugViewModel()
        {
            Drug = new Medication();
            SubstitutionType = "new";
        }


    }
}
