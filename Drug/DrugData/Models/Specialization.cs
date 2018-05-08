using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models
{
    public class Specialization
    {
        public int SpecializationId { get; set; }

        public string SpecializationName { get; set; }

        public virtual ICollection<Doctor> Doctors { get; set; }

    }
}
