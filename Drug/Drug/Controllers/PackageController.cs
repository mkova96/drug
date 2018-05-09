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
                    return RedirectToAction(nameof(Index));
                }
                _databaseContext.Package.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var ses = _databaseContext.Package
            .FirstOrDefault(p => p.PackageId == id);

            _databaseContext.Package.Remove(ses);
            _databaseContext.SaveChanges();
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

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
