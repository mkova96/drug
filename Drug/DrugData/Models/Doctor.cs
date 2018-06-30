using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DrugData.Models
{
    public class Doctor:User
    {

        public String Biography { get; set; }

        public string Education { get; set; }

        public string ImagePath   { get;set;}

    }
}
