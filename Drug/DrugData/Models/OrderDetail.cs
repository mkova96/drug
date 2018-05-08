using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public Order Order { get; set; }

        public Medication Drug { get; set; }

        public int Amount { get; set; }

        public decimal Price { get; set; }
    }

}
