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

        [StringLength(100)]
        public string PackageType { get; set; }

        public int Quantity { get; set; }

        public int IndividualSize { get; set; }

        public Measure Measure { get; set; }

        public virtual ICollection<Medication> Drugs { get; set; }

        public string MeasureName { get; set; }

        [NotMapped]
        public  virtual string PackageData => $"{PackageType}, količina u pakiranju: {Quantity}, pojedinačna veličina: {IndividualSize} {MeasureName}";
    }
}
