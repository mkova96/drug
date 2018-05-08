using System;

namespace LijekData.Models
{
    public class DrugDisease
    {
        public int DrugDiseaseId {get; set; } 
        public Drug Drug { get; set; }
        public Disease Disease { get; set; }

    }
}
