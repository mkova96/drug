using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class SearchActionViewModel
        {
           // public IEnumerable<User> Users { get; set; }
            public IEnumerable<Disease> Categories { get; set; }
            public IEnumerable<Medication> Drugs { get; set; }

            public IEnumerable<Manufacturer> Manufacturers { get; set; }

    }
}
