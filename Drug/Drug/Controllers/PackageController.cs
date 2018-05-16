using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Package> ses = _databaseContext.Package.ToList();
            return View(ses);
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new PackageViewModel());
        }

        [HttpPost]
        public IActionResult Create(PackageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ses = new Package { PackageType = model.PackageType};
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
        public IActionResult Delete(int id)
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
            return RedirectToAction(nameof(Index));
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
