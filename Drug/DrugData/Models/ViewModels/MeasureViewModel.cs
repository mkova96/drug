using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class MeasureViewModel
    {
        [Required]
        [StringLength(5)]
        public string MeasureName { get; set; }
    }
}
