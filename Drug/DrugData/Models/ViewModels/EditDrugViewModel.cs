using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditDrugViewModel
    {
        public IEnumerable<int> CategoryIds { get; set; }
        public IEnumerable<int> SideEffectIds { get; set; }

        public Medication Drug { get; set; }

        public int ManufacturerId { get; set; }
        public int PackageId { get; set; }
        public int CurrencyId { get; set; }


    }
}
