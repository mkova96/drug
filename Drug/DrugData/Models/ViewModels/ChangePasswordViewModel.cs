using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Trenutna lozinka")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Morate unijeti svoju novu lozinku")]
        [StringLength(100, ErrorMessage = "Lozinka mora imati minimalno 6 znakova te kombinaciju slova i znamenki", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova lozinka")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi novu lozinku")]
        [Compare("NewPassword", ErrorMessage = "Nova lozinka i potvrda nove lozinke se ne poklapaju")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }

    }
}
