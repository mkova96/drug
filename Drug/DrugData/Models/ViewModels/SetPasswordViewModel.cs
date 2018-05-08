using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Lozinka mora sadržavati najmanje {2} znakova, a najviše {1}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova lozinka")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdite novu lozinku")]
        [Compare("NewPassword", ErrorMessage = "Nova lozinka i potvrda se ne poklapaju.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }

    }
}
