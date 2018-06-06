using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class ManufacturerViewModel
    {
        [Required(ErrorMessage = "Naziv proizvođača je obavezan")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tekst o proizvođaču je obavezan")]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        [Required(ErrorMessage = "Poveznica slike proizvođača je obavezna")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }




    }
}
