﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LijekData.Models
{
    public class Drug
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public int ManufacturerId { get; set; }

        public DateTime DateProduced { get; set; }
        public DateTime DateExpires {get; set; }

        [DataType(DataType.MultilineText)]
        public String Usage { get; set; }

        [Range(0, 1000)]
        public int Quantity { get; set; }

        [Range(0, 10000)]
        [Required]
        public decimal Price { get; set; }
        public virtual ICollection<DrugDisease> DrugDiseases { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<DrugSideEffect> DrugSideEffects { get; set; }
        //public virtual ICollection<Drug> Substitutions { get; set; }
        //public virtual ICollection<DrugCart> DrugCarts { get; set; }

    }
}
