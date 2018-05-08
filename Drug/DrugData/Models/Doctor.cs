using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LijekData.Models
{
    public class Doctor:User
    {
        [DataType(DataType.MultilineText)]
        public String Biography { get; set; }

        public string Education { get; set; }

        public string Title { get; set; }

        public Specialization Specialization { get; set; }
    }
}
