using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditDoctorViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Morate unijeti ime")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Morate unijeti prezime")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Morate unijeti adresu")]
        [Display(Name = "Adresa")]
        public String Address { get; set; }
        public int CityId { get; set; }

        [Required(ErrorMessage = "Morate unijeti biografiju")]
        [DataType(DataType.MultilineText)]
        public String Biography { get; set; }

        [Required(ErrorMessage = "Morate unijeti obrazovanje")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Morate unijeti poveznicu slike")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Morate unijeti mail adresu")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
            ErrorMessage = "E-mail adresa je u krivom formatu")]
        public string EMail { get; set; }


    }
}
