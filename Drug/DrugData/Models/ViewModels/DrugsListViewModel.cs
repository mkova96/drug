using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class DrugsListViewModel
    {
        public IEnumerable<Medication> Drugs { get; set; }
        public string CurrentCategory { get; set; }
    }
}
