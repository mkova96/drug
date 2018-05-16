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

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<SideEffect> ses = _databaseContext.SideEffect.ToList();
            return View(ses);
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
                var ses = new SideEffect { SideEffectName = model.Name};
                var x = _databaseContext.SideEffect.FirstOrDefault(g => g.SideEffectName == ses.SideEffectName);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Nuspojava tog imena već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add));
                }
                _databaseContext.SideEffect.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Nuspojava je dodana";
                TempData[Constants.ErrorOccurred] = false;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var ses = _databaseContext.SideEffect
            .FirstOrDefault(p => p.SideEffectId == id);

            _databaseContext.SideEffect.Remove(ses);
            _databaseContext.SaveChanges();
            TempData[Constants.Message] = $"Nuspojava je obrisana";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index));
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
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditSideEffectViewModel model)
        {


            if (ModelState.IsValid)
            {
                var ses = _databaseContext.SideEffect

                .FirstOrDefault(m => m.SideEffectId == id);

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
            
        }
            return RedirectToAction(nameof(Index));
        }
    }
}
