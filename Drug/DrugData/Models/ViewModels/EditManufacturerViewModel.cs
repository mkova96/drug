using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditManufacturerViewModel
    {
        public int ManufacturerId { get; set; }

        [Required(ErrorMessage = "Naziv proizvođača je obavezan")]
        public string ManufacturerName { get; set; }

        [Required(ErrorMessage = "Tekst o proizvođaču je obavezan")]
        [DataType(DataType.MultilineText)]
        public String About { get; set; }

        [Required(ErrorMessage = "Poveznica slike proizvođača je obavezna")]
        [DataType(DataType.ImageUrl)]
        public string ImagePath { get; set; }
    }
}
