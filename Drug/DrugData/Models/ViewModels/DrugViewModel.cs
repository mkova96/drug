using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static DrugData.Models.Medication;

namespace DrugData.Models.ViewModels
{
    public class DrugViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Ime mora biti kraće od 100 znakova!")]
        public string Name { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Produced { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Expires { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }

        [Required]
        [Range(0, 10000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        //public virtual int ManufacturerId { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> CategoryIds { get; set; }

        public List<int> SideEffectIds { get; set; }

        public List<int> DrugIds { get; set; }

        [DataType(DataType.MultilineText)]
        public String Usage { get; set; }

        public string Size { get; set; }

        public string ManufacturerType { get; set; }

        public int ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public DrugViewModel()
        {
            Manufacturer = new Manufacturer();
            ManufacturerType = "existing";
        }




    }
}
