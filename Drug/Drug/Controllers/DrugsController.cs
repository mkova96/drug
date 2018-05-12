using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drug.Controllers
{
    public class DrugsController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly UserManager<User> _userManager;

        public DrugsController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, UserManager<User> userManager)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _userManager = userManager;
        }

        /* public ViewResult Index()
         {
             ViewData["Success"] = TempData["Success"];
             IEnumerable<Medication> drugs = _databaseContext.Drug.OrderBy(a=>a.DrugName).ToList();
             return View("~/Views/Drugs/Index.cshtml", new DrugsListViewModel
             {
                 Drugs = drugs
             });
         }*/

        public ViewResult Index(string category)
        {
            string _category = category;
            IEnumerable<Medication> drinks;
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(category))
            {
                drinks = _databaseContext.Drug.OrderBy(p => p.DrugId).ToList();
                currentCategory = "Svi proizvodi";
            }
            else
            {
                if (string.Equals("Po cijeni silazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Drug.OrderByDescending(p => p.Price).ToList();
                else if (string.Equals("Po cijeni uzlazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Drug.OrderBy(p => p.Price).ToList();
                else if (string.Equals("Po isteku valjanosti uzlazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Drug.OrderBy(p => p.DateExpires).ToList();
                else
                {
                    drinks = _databaseContext.Drug.OrderByDescending(p => p.DateExpires).ToList();
                }

                currentCategory = _category;
            }

            return View(new DrugsListViewModel
            {
                Drugs = drinks,
                CurrentCategory = currentCategory
            });
        }

        /*public ViewResult IndexPriceLow()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Medication> drugs = _databaseContext.Drug.OrderBy(a => a.Price).ToList();
            return View("~/Views/Drugs/Index.cshtml", new DrugsListViewModel
            {
                Drugs = drugs
            });
        }*/

        /*public ViewResult IndexPriceMax()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Medication> drugs = _databaseContext.Drug.OrderByDescending(a => a.Price).ToList();
            return View("~/Views/Drugs/Index.cshtml", new DrugsListViewModel
            {
                Drugs = drugs
            });
        }*/
    }
}
