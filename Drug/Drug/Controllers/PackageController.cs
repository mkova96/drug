using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

            var students = from s in _databaseContext.Package.Include(t=>t.Measure)
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
                Measure x;

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
                    x = _databaseContext.Measure.FirstOrDefault(g => g.MeasureName == man.MeasureName);

                    if (x != null)
                    {
                        TempData[Constants.Message] = $"Mjerna jedinica tog imena već postoji.\n";
                        TempData[Constants.ErrorOccurred] = true;
                        return RedirectToAction(nameof(Add));
                    }
                    _databaseContext.Measure.Add(man);
                }
                else
                {
                    man = _databaseContext.Measure.Find(model.MeasureId);
                }

                var ses = new Package { PackageType = model.PackageType,Quantity=model.Quantity,IndividualSize=model.IndividualSize};
                ses.Measure = man;
                ses.MeasureName = man.MeasureName;
                var z = _databaseContext.Package.FirstOrDefault(g => (g.PackageType == ses.PackageType && g.IndividualSize==ses.IndividualSize && g.Quantity==ses.Quantity && g.Measure==ses.Measure));

                System.Diagnostics.Debug.WriteLine(ses.MeasureName + ses.IndividualSize + ses.PackageType + ses.Quantity);

                if (z != null)
                {
                    TempData[Constants.Message] = $"Takvo pakiranje već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Package.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Pakiranje je dodano";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));

            }
            return View("Add", model);


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
            ViewData["Measures"] = _databaseContext.Measure.ToList();

            var ses = _databaseContext.Package.Include(t=>t.Measure)
            .FirstOrDefault(p => p.PackageId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditPackageViewModel
            {
                MeasureId = ses.Measure.MeasureId,
                Package = ses
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditPackageViewModel model)
        {
            ViewData["Measures"] = _databaseContext.Measure.ToList();
            if (id != 0)
            {
                model.Package.PackageId = id;

            }

            if (ModelState.IsValid)
            {
                    var ses = _databaseContext.Package.Include(p=>p.Measure).FirstOrDefault(m => m.PackageId == id);
                    ses.Measure = _databaseContext.Measure.ToList().First(c => c.MeasureId == model.MeasureId);

                ses.PackageType = model.Package.PackageType;
                ses.IndividualSize = model.Package.IndividualSize;
                ses.Quantity = model.Package.Quantity;
                ses.MeasureName = ses.Measure.MeasureName;

                var x = _databaseContext.Package.Where(g => (g.PackageType == ses.PackageType && g.IndividualSize == ses.IndividualSize && g.Quantity == ses.Quantity && g.MeasureName == ses.MeasureName && g.PackageId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Takvo pakiranje već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();

                TempData[Constants.Message] = $"Pakiranje je promijenjeno";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));

            }
            else
            {
                ViewData["Measures"] = _databaseContext.Measure.ToList();
                return View("Edit", model);

            }
        }
    }
}
