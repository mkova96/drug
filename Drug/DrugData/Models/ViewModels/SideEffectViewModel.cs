using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class SideEffectViewModel
    {
        [Required(ErrorMessage = "Naziv nuspojave je obavezan.")]
        public string Name { get; set; }
    }
}
