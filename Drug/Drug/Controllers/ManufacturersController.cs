using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drug.Controllers
{
    public class ManufacturersController:Controller
    {

        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly UserManager<User> _userManager;

        public ManufacturersController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, UserManager<User> userManager)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string category, int? page)
        {
            string _category = category;
            ViewData["CurrentCategory"] = category;
            IQueryable<Manufacturer> drinks;
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(category))
            {
                var students = from s in _databaseContext.Manufacturer.Include(p => p.Drugs)
                               select s;
                drinks = from s in _databaseContext.Manufacturer.OrderBy(p => p.ManufacturerId).Include(t=>t.Drugs).Where(t=>t.Drugs.Count>0) select s;
                currentCategory = "Svi proizvođači";
            }
            else
            {
                if (string.Equals("Po broju proizvoda silazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = from s in _databaseContext.Manufacturer.Include(t => t.Drugs).Where(t => t.Drugs.Count > 0).OrderByDescending(p => p.Drugs.Count) select s;
                else
                    drinks = from s in _databaseContext.Manufacturer.Include(t => t.Drugs).Where(t => t.Drugs.Count > 0).OrderBy(p => p.Drugs.Count) select s;

                currentCategory = _category;
            }

            int pageSize = 3;

            System.Diagnostics.Debug.WriteLine("Ajmooo"+ await drinks.CountAsync());

            return View(await PaginatedList<Manufacturer>.CreateAsync(drinks.AsNoTracking(), page ?? 1, pageSize));

        }

    }
}
