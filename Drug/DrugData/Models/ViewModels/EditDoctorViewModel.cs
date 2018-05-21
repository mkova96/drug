using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditDoctorViewModel
    {
        public Doctor Doctor { get; set; }
        public int CityId { get; set; }
        public string SpecializationType { get; set; }

        public Specialization Specialization { get; set; }
        public int SpecializationId { get; set; }


        public EditDoctorViewModel()
        {
            Specialization = new Specialization();
            SpecializationType = "existing";
        }
    }
}
