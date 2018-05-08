using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Ime")]
        public string Name { get; set; }

        [Display(Name = "Prezime")]
        public string Surname { get; set; }

        public string StatusMessage { get; set; }

        [Required]
        [Display(Name = "Adresa")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Poštanski broj")]
        public int PostCode { get; set; }

        [Required]
        [Display(Name = "Mjesto")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Država")]
        public int CountryID { get; set; }


    }
}
