using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Components
{
    public class CategoryMenu : ViewComponent
    {
        //private readonly ICategoryRepository _categoryRepository;
        public CategoryMenu()
        {
        }

        public IViewComponentResult Invoke()
        {
            List<String> sortList = new List<String>();
            sortList.Add("Po cijeni silazno");
            sortList.Add("Po cijeni uzlazno");
            sortList.Add("Po isteku valjanosti silazno");
            sortList.Add("Po isteku valjanosti uzlazno");

            return View(sortList);
        }
    }
}
