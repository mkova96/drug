using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditCurrencyViewModel
    {

        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "Naziv valute je obavezan.")]
        public string CurrencyName { get; set; }
    }
}
