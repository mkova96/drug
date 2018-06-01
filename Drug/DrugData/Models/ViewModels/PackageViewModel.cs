using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class PackageViewModel
    {

        [Required]
        [StringLength(20)]
        public string PackageType { get; set; }

        public int Quantity { get; set; }

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
