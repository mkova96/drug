using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Components
{
   public  class ManufacturerMenu:ViewComponent
    {
        public ManufacturerMenu()
        {
        }

        public IViewComponentResult Invoke()
        {
            List<String> sortList = new List<String>();
            sortList.Add("Po broju proizvoda silazno");
            sortList.Add("Po broju proizvoda uzlazno");

            return View(sortList);
        }
    }
}
