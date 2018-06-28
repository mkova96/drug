using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Measure
    {
        public int MeasureId { get; set; }

        [Required(ErrorMessage = "Naziv mjerne jedinice je obavezan")]
        public string MeasureName{ get; set; }
    }
}
