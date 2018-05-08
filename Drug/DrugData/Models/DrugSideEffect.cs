using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models
{
    public class DrugSideEffect
    {
        public int DrugSideEffectId { get; set; }
        public Medication Drug { get; set; }
        public SideEffect SideEffect { get; set; }
    }
}
