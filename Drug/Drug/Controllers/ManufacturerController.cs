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
    public class ManufacturerController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ManufacturerController(ApplicationDbContext context,
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

            var students = from s in _databaseContext.Manufacturer.Include(p=>p.Drugs)
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.ManufacturerName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.ManufacturerName);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.ManufacturerId);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.ManufacturerId);
                    break;
                default:
                    students = students.OrderBy(s => s.ManufacturerName);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Manufacturer>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new ManufacturerViewModel());
        }

        [HttpPost]
        public IActionResult Create(ManufacturerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var man = new Manufacturer { ManufacturerName = model.Name,About=model.About };
                var x = _databaseContext.Manufacturer.FirstOrDefault(g => g.ManufacturerName == man.ManufacturerName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Proizvođač tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Manufacturer.Add(man);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Proizvođač je dodan";
                TempData[Constants.ErrorOccurred] = false;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id, int? page)
        {
            var man = _databaseContext.Manufacturer
            .FirstOrDefault(p => p.ManufacturerId == id);

            try
            {
                _databaseContext.Manufacturer.Remove(man);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Proizvođač je obrisan";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Proizvođača nije moguće obrisati jer postoje lijekovi koji ga sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }
            var x = _databaseContext.Manufacturer.ToList().Count;

            if ((page - 1) * 8 == x && page != 1)
            {
                --page;
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var man = _databaseContext.Manufacturer
            .FirstOrDefault(p => p.ManufacturerId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditManufacturerViewModel
            {
                Manufacturer = man
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditManufacturerViewModel model)
        {


            if (ModelState.IsValid)
            {
                var man = _databaseContext.Manufacturer.FirstOrDefault(m => m.ManufacturerId == id);

                man.ManufacturerName = model.Manufacturer.ManufacturerName;
                man.About = model.Manufacturer.About;

                var x = _databaseContext.Manufacturer.Where(g => (g.ManufacturerName == man.ManufacturerName && g.ManufacturerId!=id)).ToList();
                if (x.Count>0)
                {
                    TempData[Constants.Message] = $"Proizvođač tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Proizvođač je promijenjen";
                TempData[Constants.ErrorOccurred] = false;
            }
            return RedirectToAction(nameof(Index));
        }

        public ViewResult Show(int id)
        {
            Manufacturer manufacturer = _databaseContext.Manufacturer.FirstOrDefault(m => m.ManufacturerId == id);
            return View(manufacturer);
        }



    }







}

