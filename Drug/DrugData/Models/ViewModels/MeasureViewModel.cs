using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class MeasureViewModel
    {
        [Required(ErrorMessage = "Naziv mjerne jedinice je obavezan")]
        public string MeasureName { get; set; }
    }
}
