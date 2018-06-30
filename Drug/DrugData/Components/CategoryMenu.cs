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
            sortList.Add("Po cijeni ▼");
            sortList.Add("Po cijeni ▲");
            sortList.Add("Po isteku valjanosti ▼");
            sortList.Add("Po isteku valjanosti ▲");

            return View(sortList);
        }
    }
}
