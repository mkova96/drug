using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class SideEffectViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
