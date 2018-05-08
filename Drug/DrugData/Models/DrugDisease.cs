using System;

namespace DrugData.Models
{
    public class DrugDisease
    {
        public int DrugDiseaseId {get; set; } 
        public Medication Drug { get; set; }
        public Disease Disease { get; set; }

    }
}
