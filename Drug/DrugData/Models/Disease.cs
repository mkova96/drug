using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Disease
    {
        public int DiseaseId { get; set; }
        public string DiseaseName { get; set; }
        public string ICD { get; set; }

        public virtual ICollection<DrugDisease> DrugDiseases { get; set; }
    }
}
