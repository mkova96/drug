using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Morate unijeti ime admina")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Morate unijeti prezime admina")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Morate unijeti mail adresu admina")]
        public string EMail { get; set; }


        [Required(ErrorMessage = "Morate unijeti lozinku za admina")]
        [StringLength(100, ErrorMessage = "Lozinka mora imati minimalno 6 znakova te kombinaciju slova i znamenki", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Morate unijeti adresu admina")]
        [StringLength(100)]
        [Display(Name = "Adresa")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Morate unijeti grad admina")]
        [Display(Name = "Grad")]
        public int CityId { get; set; }

    }
}
