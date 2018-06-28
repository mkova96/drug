using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DrugData.Models
{
    public class Package
    {
        public int PackageId { get; set; }

        [Required(ErrorMessage = "Naziv pakiranja je obavezan")]
        public string PackageType { get; set; }

        [Required(ErrorMessage = "Količina unutar pakiranja je obavezna")]
        [Range(1, 1000, ErrorMessage = "Količina unutar pakiranja mora biti veća od 0")]
        public int Quantity { get; set; }

        [Range(1, 1000, ErrorMessage = "Veličina pojedinačne stavke unutar pakiranja mora biti veća od 0")]
        public int IndividualSize { get; set; }

        public Measure Measure { get; set; }

        public virtual ICollection<Medication> Drugs { get; set; }

        public string MeasureName { get; set; }

        [NotMapped]
        public  virtual string PackageData => $"{PackageType}, količina u pakiranju: {Quantity}, pojedinačna veličina: {IndividualSize} {MeasureName}";
    }
}
