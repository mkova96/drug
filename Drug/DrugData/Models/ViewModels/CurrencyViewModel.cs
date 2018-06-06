using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class CurrencyViewModel
    {
        [Required(ErrorMessage = "Naziv valute je obavezan.")]
        public string CurrencyName { get; set; }
    }
}
