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

        [Required]
        [StringLength(3)]
        public string CurrencyName { get; set; }

    }
}
