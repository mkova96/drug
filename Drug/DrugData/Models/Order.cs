using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public List<OrderDetail> OrderLines { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public decimal PriceSum { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Unesite svoje ime")]
        [Display(Name = "Ime")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Unesite svoje prezime")]
        [Display(Name = "Prezime")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Unesite svoju adresu")]
        [StringLength(100)]
        [Display(Name = "Adresa")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Unesite svoj poštanski broj")]
        [Display(Name = "Poštanski broj")]
        [StringLength(10, MinimumLength = 4)]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Unesite svoj grad")]
        [Display(Name = "Grad")]
        [StringLength(100)]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Unesite svoju državu")]
        [Display(Name = "Država")]
        [StringLength(50)]
        public string CountryName { get; set; }



        [Required]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
            ErrorMessage = "E-mail adresa je u krivom formatu")]
        public string Email { get; set; }

    }

}
