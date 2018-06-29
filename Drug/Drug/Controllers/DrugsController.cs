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

        public async Task<IActionResult> Index(string category, int? page, string[] diseases)
        {
            string _category = category;
            IEnumerable<Medication> drinks;
            ViewData["CurrentCategory"] = category;
            ViewData["Filteri"] = diseases;


            string currentCategory = string.Empty;

            var applicationDbContext = _databaseContext.Drug.OrderBy(p => p.DrugId)
                   .Where(p => p.Quantity > 0)
                .Include(r => r.Currency).Include(i => i.Package).ThenInclude(t => t.Measure)
                .Include(p => p.Manufacturer).Include(e => e.DrugDiseases)
                .ThenInclude(eu => eu.Disease).ToList();

            HashSet<Medication> meds = applicationDbContext.ToHashSet();

            System.Diagnostics.Debug.WriteLine("VELICINA"+diseases.Count());


            if (diseases.Length != 0)
            {
                System.Diagnostics.Debug.WriteLine("USLO");

                HashSet<Medication> ba = new HashSet<Medication>();
                foreach (var i in meds)
                {
                    ba.Add(i);
                }
                foreach (var e in meds)
                {
                    int check = 0;

                    foreach (var p in diseases)
                    {
                        if (e.DrugDiseases.Any(g => g.Disease.DiseaseName.Equals(p)))
                        {
                            check = 1;
                        }
                    }
                    if (check == 0)
                    {
                        ba.Remove(e);
                    }
                }
                System.Diagnostics.Debug.WriteLine("MEDS" + ba.Count);
                meds = ba;
            }

            drinks = meds.ToList();



            if (string.IsNullOrEmpty(category))
            {
                System.Diagnostics.Debug.WriteLine("tu");
                drinks = meds.ToList();

                currentCategory = "Svi proizvodi";
            }
            else
            {
                if (string.Equals("Po cijeni silazno", _category, StringComparison.OrdinalIgnoreCase))
                drinks = meds.ToList().OrderByDescending(p => p.Price);

                else if (string.Equals("Po cijeni uzlazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = meds.ToList().OrderBy(p => p.Price);

                else if (string.Equals("Po isteku valjanosti uzlazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = meds.ToList().OrderBy(p => p.DateExpires);

                else
                {
                    drinks = meds.ToList().OrderByDescending(p => p.DateExpires);

                }

                currentCategory = _category;
            }

            int pageSize = 9;
            return View(await PaginatedList2<Medication>.CreateAsync(drinks, page ?? 1, pageSize));
        }

      
    }
}
