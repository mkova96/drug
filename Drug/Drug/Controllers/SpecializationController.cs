using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class SpecializationController : Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public SpecializationController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Specialization> ses = _databaseContext.Specialization.ToList();
            return View(ses);
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new SpecializationViewModel());
        }

        [HttpPost]
        public IActionResult Create(SpecializationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ses = new Specialization { SpecializationName = model.Name };
                var x = _databaseContext.Specialization.FirstOrDefault(g => g.SpecializationName == ses.SpecializationName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Specijalizacija tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.Specialization.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Specijalizacija je dodana";
                TempData[Constants.ErrorOccurred] = false;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var ses = _databaseContext.Specialization
            .FirstOrDefault(p => p.SpecializationId == id);

            try
            {
                _databaseContext.Specialization.Remove(ses);
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Specijalizacija je obrisana";
                TempData[Constants.ErrorOccurred] = false;
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = $"Specijalizaciju nije moguće obrisati jer postoje doktori koju ju sadrže.";
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var ses = _databaseContext.Specialization
            .FirstOrDefault(p => p.SpecializationId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditSpecializationViewModel
            {
                Specialization = ses
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditSpecializationViewModel model)
        {


            if (ModelState.IsValid)
            {
                var ses = _databaseContext.Specialization

                .FirstOrDefault(m => m.SpecializationId == id);

                ses.SpecializationName = model.Specialization.SpecializationName;
                var x = _databaseContext.Specialization.Where(g => (g.SpecializationName == ses.SpecializationName && g.SpecializationId != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Specijalizacija tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Specijalizacija je promijenjena";
                TempData[Constants.ErrorOccurred] = false;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
