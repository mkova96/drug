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

        public int PackageId { get; set; }
        public int CurrencyId { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public bool z { get; set; }
        public List<int> DrugIds { get; set; }

        public string ManufacturerType { get; set; }

        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        public EditDrugViewModel()
        {
            Manufacturer = new Manufacturer();
            ManufacturerType = "existing";
        }


    }
}
