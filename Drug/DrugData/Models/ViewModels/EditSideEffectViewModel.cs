using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditSideEffectViewModel
    {
        public int SideEffectId { get; set; }

        [Required(ErrorMessage = "Ime nuspojave je nužno.")]
        public string SideEffectName { get; set; }
    }
}
