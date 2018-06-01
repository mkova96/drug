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

namespace Drug.Controllers
{
    public class PackageController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public PackageController(ApplicationDbContext context,
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

            var students = from s in _databaseContext.Package
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.PackageType.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.PackageType);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.PackageId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.PackageId);
                    break;
                default:
                    students = students.OrderBy(s => s.PackageType);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Package>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add()
        {
            ViewData["Measures"] = _databaseContext.Measure.ToList();

            return View(new PackageViewModel());
        }

        [HttpPost]
        public IActionResult Create(PackageViewModel model)
        {
            ViewData["Measures"] = _databaseContext.Measure.ToList();

            if (ModelState.IsValid)
            {
                Measure man;

                if (model.MeasureType == "new")
                {
                    // Additional validation before creating the Company
                    var requiredFields = new[]
                    {
                        new Tuple<string, object>("Name", model.Measure.MeasureName),

                    };

                    foreach (var field in requiredFields)
                    {
                        if (field.Item2 == null || field.Item2.Equals(""))
                        {
                            ModelState.AddModelError(string.Empty, $"{field.Item1} field is required.");
                        }
                    }
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    man = model.Measure;
                    _databaseContext.Measure.Add(man);
                }
                else
                {
                    man = _databaseContext.Measure.Find(model.MeasureId);
                }

                var ses = new Package { PackageType = model.PackageType,Quantity=model.Quantity,IndividualSize=model.IndividualSize};
                ses.Measure = man;
                var x = _databaseContext.Package.FirstOrDefault(g => g.PackageType == ses.PackageType);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Pakiranje tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Package.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Pakiranje je dodano";
                TempData[Constants.ErrorOccurred] = false;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id, int? page)
        {
            var ses = _databaseContext.Package
            .FirstOrDefault(p => p.PackageId == id);

            try
            {
                _databaseContext.Package.Remove(ses);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Pakiranje je obrisano";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Pakiranje nije moguće obrisati jer postoje lijekovi koji ga sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }
            var x = _databaseContext.Package.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var ses = _databaseContext.Package
            .FirstOrDefault(p => p.PackageId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditPackageViewModel
            {
                Package = ses
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditPackageViewModel model)
        {


            if (ModelState.IsValid)
            {
                var ses = _databaseContext.Package

                .FirstOrDefault(m => m.PackageId == id);

                ses.PackageType = model.Package.PackageType;

                var x = _databaseContext.Package.Where(g => (g.PackageType == ses.PackageType && g.PackageId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Pakiranje tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Pakiranje je promijenjeno";
                TempData[Constants.ErrorOccurred] = false;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
