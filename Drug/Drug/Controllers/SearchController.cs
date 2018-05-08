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
            IEnumerable<Disease> categories =
                _databaseContext.Disease.Where(c => searchForArray(c.DiseaseName.ToLower(), searchLower)).ToList();
            IEnumerable<Medication> drugs =
                _databaseContext.Drug.Where(c => searchForArray(c.DrugName.ToLower(), searchLower)).ToList();
            IEnumerable<Manufacturer> manufacturers =
                _databaseContext.Manufacturer.Where(c => searchForArray(c.ManufacturerName.ToLower(), searchLower)).ToList();

            /* IEnumerable<User> users = _databaseContext.Users.Where(u =>
                 searchForArray(u.UserName.ToLower(), searchLower) || searchForArray(u.Name.ToLower(), searchLower) ||
                 searchForArray(u.Surname.ToLower(), searchLower)).ToList();*/


            var searchResults = new SearchActionViewModel
            {
               // Users = users,
                Categories = categories,
                Drugs=drugs,
                Manufacturers=manufacturers
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
