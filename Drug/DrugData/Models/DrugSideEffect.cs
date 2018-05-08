using System;
using System.Collections.Generic;
using System.Text;

namespace LijekData.Models
{
    public class DrugSideEffect
    {
        public int DrugSideEffectId { get; set; }
        public Drug Drug { get; set; }
        public SideEffect SideEffect { get; set; }
    }
}
