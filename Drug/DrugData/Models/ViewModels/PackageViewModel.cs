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


    }
}
