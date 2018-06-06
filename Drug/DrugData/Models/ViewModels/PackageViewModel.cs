using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class PackageViewModel
    {

        [Required(ErrorMessage = "Naziv pakiranja je obavezan")]
        public string PackageType { get; set; }

        [Range(1, 1000, ErrorMessage = "Količina unutar pakiranja mora biti veća od 0")]
        public int Quantity { get; set; }

        [Range(1, 1000, ErrorMessage = "Veličina pojedinačne stavke unutar pakiranja mora biti veća od 0")]
        public int IndividualSize { get; set; }

        public string MeasureType { get; set; }

        public int MeasureId { get; set; }
        public Measure Measure { get; set; }

        public PackageViewModel()
        {
            Measure = new Measure();
            MeasureType = "existing";
        }


    }
}
