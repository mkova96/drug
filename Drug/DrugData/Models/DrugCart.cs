using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class DrugCart
    {
        public int DrugCartId {get;set;}

        public Cart Cart { get; set; }
        public Medication Drug { get; set; }

        [Required]
        public int Quantity { get; set; }

    }
}
