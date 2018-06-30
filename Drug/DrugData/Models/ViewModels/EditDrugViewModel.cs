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

        public int DrugId { get; set; }

        [Required(ErrorMessage = "Naziv proizvoda je obavezan")]
        public string DrugName { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public Package Package { get; set; }

        [Required(ErrorMessage = "Datum proizvodnje proizvoda je obavezan")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateProduced { get; set; }

        [Required(ErrorMessage = "Datum isteka valjanosti proizvoda je obavezan")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateExpires { get; set; }

        [Required(ErrorMessage = "Poveznica slike proizvoda je obavezna")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Način uporabe proizvoda je obavezan")]
        [DataType(DataType.MultilineText)]
        public String Usage { get; set; }

        [Range(1, 1000, ErrorMessage = "Količina proizvoda na skladištu mora biti veća od 0")]
        public int Quantity { get; set; }

        [Range(1, 10000, ErrorMessage = "Cijena mora biti veća od 0")]
        public decimal Price { get; set; }


    }
}
