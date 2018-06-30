using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditPackageViewModel
    {
        public int MeasureId { get; set; }

        public int PackageId { get; set; }

        [Required(ErrorMessage = "Naziv pakiranja je obavezan")]
        public string PackageType { get; set; }

        [Required(ErrorMessage = "Količina unutar pakiranja je obavezna")]
        [Range(1, 1000, ErrorMessage = "Količina unutar pakiranja mora biti veća od 0")]
        public int Quantity { get; set; }

        [Range(1, 1000, ErrorMessage = "Veličina pojedinačne stavke unutar pakiranja mora biti veća od 0")]
        public int IndividualSize { get; set; }


    }
}
