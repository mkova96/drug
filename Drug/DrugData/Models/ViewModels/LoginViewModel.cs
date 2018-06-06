using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Morate unijeti svoju mail adresu")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Morate unijeti svoju lozinku")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Zapamti me?")]
        public bool RememberMe { get; set; }

    }
}
