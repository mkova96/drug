using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DrugData.Models
{
    public class Doctor:User
    {
        [Required(ErrorMessage = "Morate unijeti biografiju")]
        [DataType(DataType.MultilineText)]
        public String Biography { get; set; }

        [Required(ErrorMessage = "Morate unijeti obrazovanje")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Morate unijeti poveznicu slike")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath   { get;set;}

    }
}
