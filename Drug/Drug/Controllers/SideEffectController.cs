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
                _databaseContext.SideEffect.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
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

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
