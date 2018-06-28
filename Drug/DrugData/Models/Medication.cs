using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DrugData.Models
{
    public class Medication
    {
        [Key]
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
        public DateTime DateExpires {get; set; }

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

        public Currency Currency { get; set; }
        public virtual ICollection<DrugDisease> DrugDiseases { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<DrugSideEffect> DrugSideEffects { get; set; }
        public virtual ICollection<Medication> Substitutions { get; set; }
        public virtual ICollection<DrugCart> DrugCarts { get; set; }

    }
}
