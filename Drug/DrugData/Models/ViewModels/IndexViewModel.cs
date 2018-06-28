using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        [Required(ErrorMessage = "Morate unijeti mail adresu")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Morate unijeti ime")]
        [Display(Name = "Ime")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Morate unijeti prezime")]
        [Display(Name = "Prezime")]
        public string Surname { get; set; }

        public string StatusMessage { get; set; }

        [Required(ErrorMessage = "Morate unijeti adresu")]
        [Display(Name = "Adresa")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Grad")]
        public int CityId { get; set; }


    }
}
