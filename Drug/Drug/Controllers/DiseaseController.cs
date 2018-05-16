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

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Disease> cats = _databaseContext.Disease.ToList();
            return View(cats);
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
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var cat = _databaseContext.Disease
            .FirstOrDefault(p => p.DiseaseId == id);

            _databaseContext.Disease.Remove(cat);
            _databaseContext.SaveChanges();
            TempData[Constants.Message] = $"Bolest je obrisana";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index));
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
            }
            return RedirectToAction(nameof(Index));
        }

        public ViewResult Show(int id)
        {
            Disease category = _databaseContext.Disease.FirstOrDefault(m => m.DiseaseId == id);
            return View(category);
        }
    }
}
