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

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Manufacturer> manufacturers = _databaseContext.Manufacturer.Include(t=>t.Drugs).ToList();
            return View(manufacturers);
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
        public IActionResult Delete(int id)
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
            return RedirectToAction(nameof(Index));
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

