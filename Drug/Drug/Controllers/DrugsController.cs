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
        public ViewResult Show(int id)
        {
            Medication drug = _databaseContext.Drug.Include(t => t.Comments).ThenInclude(p => p.User)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect).Where(p=>p.Quantity>0)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure).Include(p => p.Substitutions)
                .Include(p => p.Manufacturer).Include(e => e.DrugDiseases)
                .ThenInclude(eu => eu.Disease).FirstOrDefault(m => m.DrugId == id);
            return View(drug);
        }

        public ViewResult Index(string category)
        {
            string _category = category;
            IEnumerable<Medication> drinks;
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(category))
            {
                drinks = _databaseContext.Drug.OrderBy(p => p.DrugId).
                    Include(t => t.Comments).ThenInclude(p => p.User)
                .Include(p => p.DrugSideEffects).ThenInclude(i => i.SideEffect).Where(p => p.Quantity > 0)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure).Include(p => p.Substitutions)
                .Include(p => p.Manufacturer).Include(e => e.DrugDiseases)
                .ThenInclude(eu => eu.Disease).ToList();
                currentCategory = "Svi proizvodi";
            }
            else
            {
                if (string.Equals("Po cijeni silazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Drug.OrderByDescending(p => p.Price).Where(p => p.Quantity > 0).ToList();
                else if (string.Equals("Po cijeni uzlazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Drug.OrderBy(p => p.Price).Where(p => p.Quantity > 0).ToList();
                else if (string.Equals("Po isteku valjanosti uzlazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Drug.OrderBy(p => p.DateExpires).Where(p => p.Quantity > 0).ToList();
                else
                {
                    drinks = _databaseContext.Drug.OrderByDescending(p => p.DateExpires).Where(p => p.Quantity > 0).ToList();
                }

                currentCategory = _category;
            }

            return View(new DrugsListViewModel
            {
                Drugs = drinks,
                CurrentCategory = currentCategory
            });
        }

    }
}
