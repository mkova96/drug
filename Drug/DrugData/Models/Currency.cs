using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DrugData.Models
{
    public class Currency
    {
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "Naziv valute je obavezan.")]
        public string CurrencyName { get; set; }

    }
}
