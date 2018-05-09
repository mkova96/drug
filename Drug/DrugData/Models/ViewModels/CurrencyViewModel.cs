using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class CurrencyViewModel
    {
        [Required]
        [StringLength(3)]
        public string CurrencyName { get; set; }
    }
}
