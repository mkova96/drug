using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Components
{
    public class DoctorMenu:ViewComponent
    {
        public DoctorMenu()
        {
        }

        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
