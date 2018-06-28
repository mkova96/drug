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

namespace Lijek.Controllers
{
   // [Authorize(Roles = "Admin")]

    public class SideEffectController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public SideEffectController(ApplicationDbContext context,
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

            var students = from s in _databaseContext.SideEffect
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.SideEffectName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.SideEffectName);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.SideEffectId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.SideEffectId);
                    break;
                default:
                    students = students.OrderBy(s => s.SideEffectName);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<SideEffect>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new SideEffectViewModel());
        }

        [HttpPost]
        public IActionResult Create(SideEffectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sideEffect = new SideEffect { SideEffectName = model.Name};
                var x = _databaseContext.SideEffect.FirstOrDefault(g => g.SideEffectName == sideEffect.SideEffectName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Nuspojava tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.SideEffect.Add(sideEffect);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Nuspojava je dodana";
                TempData[Constants.ErrorOccurred] = false;

                return RedirectToAction(nameof(Index));

            }
            else
            {
                return View("Add", model);

            }

        }

        [HttpPost]
        public IActionResult Delete(int id, int? page)
        {
            var ses = _databaseContext.SideEffect
            .FirstOrDefault(p => p.SideEffectId == id);
            try
            {
                _databaseContext.SideEffect.Remove(ses);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Nuspojava je obrisana";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch(Exception exc)
            {
                TempData[Constants.Message] = $"Nuspojavu nije moguće obrisati jer postoje lijekovi koju ju sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }
            var x = _databaseContext.SideEffect.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var ses = _databaseContext.SideEffect
            .FirstOrDefault(p => p.SideEffectId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditSideEffectViewModel
            {
                SideEffect = ses
            };
            System.Diagnostics.Debug.WriteLine("ID" + ses.SideEffectId);

            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditSideEffectViewModel model)
        {
            if (id != 0)
            {
                model.SideEffect.SideEffectId = id;

            }

            if (ModelState.IsValid)
            {

                if (id == model.SideEffect.SideEffectId)
                {
                    System.Diagnostics.Debug.WriteLine("YEEEA");

                }

                var ses = _databaseContext.SideEffect.FirstOrDefault(m => m.SideEffectId == id);
                ses.SideEffectName = model.SideEffect.SideEffectName;

                var x = _databaseContext.SideEffect.Where(g => (g.SideEffectName == ses.SideEffectName && g.SideEffectId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Nuspojava tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Nuspojava je promijenjena";
                TempData[Constants.ErrorOccurred] = false;

                return RedirectToAction(nameof(Index));


            }
            else
            {
                return View("Edit",model);
            }


        }

        
    }
}
