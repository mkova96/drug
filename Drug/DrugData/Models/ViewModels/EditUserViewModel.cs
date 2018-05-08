using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditUserViewModel
    {
        public User User { get; set; }

        public int PostCode { get; set; }

        public int CountryId { get; set; }
        public int CityId { get; set; }



    }
}
