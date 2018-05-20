using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class ManufacturersListViewModel
    {
        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        public string CurrentCategory { get; set; }
    }
}
