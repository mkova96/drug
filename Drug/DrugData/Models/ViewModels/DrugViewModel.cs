using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static DrugData.Models.Medication;

namespace DrugData.Models.ViewModels
{
    public class DrugViewModel 
    {
        [Required(ErrorMessage = "Naziv proizvoda je obavezan")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Poveznica slike proizvoda je obavezna")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Datum proizvodnje proizvoda je obavezan")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Produced { get; set; }

        [Required(ErrorMessage = "Datum isteka valjanosti proizvoda je obavezan")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Expires { get; set; }

        [Range(1, 1000, ErrorMessage = "Količina proizvoda na skladištu mora biti veća od 0")]
        public int Stock { get; set; }

        [Range(0, 10000, ErrorMessage = "Cijena mora biti veća od 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Bolesti na koje je proizvod primjenjiv su obavezne")]
        [MinLength(1)]
        public List<int> CategoryIds { get; set; }

        [Required(ErrorMessage = "Moguće nuspojave proizvoda su obavezne")]
        [MinLength(1)]
        public List<int> SideEffectIds { get; set; }


        public List<int> DrugIds { get; set; }

        [Required(ErrorMessage = "Način uporabe proizvoda je obavezan")]
        [DataType(DataType.MultilineText)]
        public String Usage { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public bool z { get; set; }
        public string ManufacturerType { get; set; }
        public Manufacturer Manufacturer { get; set; }

        public int ManufacturerId { get; set; }
        public int MeasureId { get; set; }


        public string PackageType { get; set; }
        public Package Package { get; set; }

        public int PackageId { get; set; }
        public int CurrencyId { get; set; }

        public Medication Drug { get; set; }
        public string SubstitutionType { get; set; }


        public DrugViewModel()
        {
            Manufacturer = new Manufacturer();
            ManufacturerType = "existing";

            Package = new Package();
            PackageType = "existing";

            Drug = new Medication();
            SubstitutionType = "new";


        }

    }
}
