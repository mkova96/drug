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

    public class MeasureController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public MeasureController(ApplicationDbContext context,
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

            var students = from s in _databaseContext.Measure
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.MeasureName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.MeasureName);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.MeasureId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.MeasureId);
                    break;
                default:
                    students = students.OrderBy(s => s.MeasureName);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Measure>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new MeasureViewModel());
        }

        [HttpPost]
        public IActionResult Create(MeasureViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ses = new Measure { MeasureName = model.MeasureName };
                var x = _databaseContext.Measure.FirstOrDefault(g => g.MeasureName == ses.MeasureName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Mjerna jedinica tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return View("Add", model);
                }
                _databaseContext.Measure.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Mjerna jedinica je dodana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));

            }
            return View("Add", model);


        }

        [HttpPost]
        public IActionResult Delete(int id, int? page)
        {
            var ses = _databaseContext.Measure
            .FirstOrDefault(p => p.MeasureId == id);

            try
            {
                _databaseContext.Measure.Remove(ses);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Mjerna jedinica je obrisana";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Mjernu jedinicu nije moguće obrisati jer postoje pakiranja koji ju sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }
            var x = _databaseContext.Measure.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var ses = _databaseContext.Measure
            .FirstOrDefault(p => p.MeasureId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditMeasureViewModel
            {
                MeasureId=ses.MeasureId,
                MeasureName=ses.MeasureName
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditMeasureViewModel model)
        {
            if (id != 0)
            {
                model.MeasureId = id;

            }

            if (ModelState.IsValid)
            {
                var ses = _databaseContext.Measure

                .FirstOrDefault(m => m.MeasureId == id);

                ses.MeasureName = model.MeasureName;

                var x = _databaseContext.Measure.Where(g => (g.MeasureName == ses.MeasureName && g.MeasureId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Mjerna jedinica tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return View("Edit", model);
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Mjerna jedinica je promijenjena";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View("Edit", model);

            }
        }
    }
}
