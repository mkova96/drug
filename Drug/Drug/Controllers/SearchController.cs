using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Lijek.Controllers
{
    public class SearchController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;


        public SearchController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public ViewResult SearchAction(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) {
                return View();
            }

            var searchLower = searchString.ToLower().Split(" ");
            var  drugs =
                _databaseContext.Drug.Include(i => i.Package).ThenInclude(t => t.Measure).Where(c => searchForArray(c.DrugName.ToLower(), searchLower)).ToList();


            var searchResults = new SearchActionViewModel
            {
                Drugs=drugs
            };
            return View(searchResults);
        }

        private bool searchForArray(string value, string[] searchStrings)
        {
            foreach (var s in searchStrings)
            {
                if (value.Contains(s)) return true;
            }
            return false;
        }


    }
}
