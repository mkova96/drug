using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class DoctorViewModel
    {
        [Required(ErrorMessage = "Morate unijeti ime ljekarnika")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Morate unijeti prezime ljekarnika")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Morate unijeti mail adresu ljekarnika")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "Morate unijeti lozinku za ljekarnika")]
        [StringLength(100, ErrorMessage = "Lozinka mora imati minimalno 6 znakova te kombinaciju slova i znamenki", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Morate unijeti ime biografiju ljekarnika")]
        [DataType(DataType.MultilineText)]
        public String About { get; set; }

        [Required(ErrorMessage = "Morate unijeti obrazovanje ljekarnika")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Morate unijeti adresu ljekarnika")]
        [Display(Name = "Adresa")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Morate unijeti grad ljekarnika")]
        [Display(Name = "Grad")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Morate unijeti poveznicu slike ljekarnika")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }


    }
}
