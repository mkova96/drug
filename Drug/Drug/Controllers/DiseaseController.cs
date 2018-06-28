using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public DiseaseController(ApplicationDbContext context,
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

            var students = from s in _databaseContext.Disease
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.DiseaseName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.DiseaseName);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.DiseaseId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.DiseaseId);
                    break;
                default:
                    students = students.OrderBy(s => s.DiseaseName);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Disease>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new CategoryViewModel());
        }

        [HttpPost]
        public IActionResult Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cat = new Disease { DiseaseName = model.Name, ICD = model.MKB };

                var x = _databaseContext.Disease.FirstOrDefault(g => g.DiseaseName == cat.DiseaseName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Bolest tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Disease.Add(cat);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Bolest je dodana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));

            }
            return View("Add", model);

        }

        [HttpPost]
        public IActionResult Delete(int id, int? page)
        {
            var cat = _databaseContext.Disease
            .FirstOrDefault(p => p.DiseaseId == id);

            try
            {
                _databaseContext.Disease.Remove(cat);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Bolest je obrisana";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Bolest nije moguće obrisati jer postoje lijekovi koju ju sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }
            var x = _databaseContext.Disease.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var cat = _databaseContext.Disease
            .FirstOrDefault(p => p.DiseaseId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditCategoryViewModel
            {
                Category = cat
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditCategoryViewModel model)
        {
            if (id != 0)
            {
                model.Category.DiseaseId = id;

            }

            if (ModelState.IsValid)
            {
                var cat = _databaseContext.Disease

                .FirstOrDefault(m => m.DiseaseId == id);

                cat.DiseaseName = model.Category.DiseaseName;
                cat.ICD = model.Category.ICD;

                var x = _databaseContext.Disease.Where(g => (g.DiseaseName == cat.DiseaseName && g.DiseaseId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Bolest tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Bolest je promijenjena";
                TempData[Constants.ErrorOccurred] = false;

                return RedirectToAction(nameof(Index));

            }
            return View("Edit", model);

        }

        public ViewResult Show(int id)
        {
            Disease category = _databaseContext.Disease.FirstOrDefault(m => m.DiseaseId == id);
            return View(category);
        }
    }
}
