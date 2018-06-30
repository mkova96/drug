using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drug.Controllers
{
    [Authorize(Roles = "Admin")]

    public class CurrencyController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public CurrencyController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "Id_desc" : "Id";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var students = from s in _databaseContext.Currency
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.CurrencyName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.CurrencyName);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.CurrencyId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.CurrencyId);
                    break;
                default:
                    students = students.OrderBy(s => s.CurrencyName);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Currency>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new CurrencyViewModel());
        }

        [HttpPost]
        public IActionResult Create(CurrencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ses = new Currency { CurrencyName = model.CurrencyName};
                var x = _databaseContext.Currency.FirstOrDefault(g => g.CurrencyName == ses.CurrencyName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Valuta tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Currency.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Valuta je dodana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));

            }
            return View("Add", model);


        }

        [HttpPost]
        public IActionResult Delete(int id, int? page)
        {
            var ses = _databaseContext.Currency
            .FirstOrDefault(p => p.CurrencyId == id);

            try
            {
                _databaseContext.Currency.Remove(ses);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Valuta je obrisana";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Valutu nije moguće obrisati jer postoje lijekovi koju ju sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }

            var x = _databaseContext.Currency.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var ses = _databaseContext.Currency
            .FirstOrDefault(p => p.CurrencyId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditCurrencyViewModel
            {
                CurrencyId = ses.CurrencyId,
                CurrencyName=ses.CurrencyName
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditCurrencyViewModel model)
        {
            if (id != 0)
            {
                model.CurrencyId = id;

            }

            if (ModelState.IsValid)
            {
                var ses = _databaseContext.Currency

                .FirstOrDefault(m => m.CurrencyId == id);

                ses.CurrencyName = model.CurrencyName;

                var x = _databaseContext.Currency.Where(g => (g.CurrencyName == ses.CurrencyName && g.CurrencyId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Valuta tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Valuta je promijenjena";
                TempData[Constants.ErrorOccurred] = false;

                return RedirectToAction(nameof(Index));

            }
            return View("Edit", model);

        }
    }
}
