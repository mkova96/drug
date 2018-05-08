using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            IEnumerable<Manufacturer> manufacturers = _databaseContext.Manufacturer.ToList();
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
                _databaseContext.Manufacturer.Add(man);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var man = _databaseContext.Manufacturer
            .FirstOrDefault(p => p.ManufacturerId == id);

            _databaseContext.Manufacturer.Remove(man);
            _databaseContext.SaveChanges();
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
                var man = _databaseContext.Manufacturer

                .FirstOrDefault(m => m.ManufacturerId == id);

                man.ManufacturerName = model.Manufacturer.ManufacturerName;
                man.About = model.Manufacturer.About;

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
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

