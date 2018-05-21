using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class DoctorViewModel
    {
        [Required(ErrorMessage = "Morate unijeti ime korisnika")]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string EMail { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.MultilineText)]
        public String About { get; set; }

        public string Education { get; set; }

        public string Title { get; set; }

        public int SpecializationId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Adresa")]
        public string Address { get; set; }


        [Required]
        [Display(Name = "Grad")]
        public int CityId { get; set; }

        public string SpecializationType { get; set; }

        public Specialization Specialization { get; set; }

        public DoctorViewModel()
        {
            Specialization = new Specialization();
            SpecializationType = "existing";
        }

    }
}
