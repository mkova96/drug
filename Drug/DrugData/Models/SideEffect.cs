using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class SideEffect
    {
        public int SideEffectId { get; set; }

        public virtual ICollection<DrugSideEffect> DrugSideEffect { get; set; }

        public string SideEffectName { get; set; }
    }
}
