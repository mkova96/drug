﻿using DrugData;
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
    public class CurrencyController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public CurrencyController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Currency> ses = _databaseContext.Currency.ToList();
            return View(ses);
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new CurrencyViewModel());
        }

        [HttpPost]
        public IActionResult Create(CurrencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ses = new Currency { CurrencyName = model.CurrencyName };
                var x = _databaseContext.Currency.FirstOrDefault(g => g.CurrencyName == ses.CurrencyName);

                if (x != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                _databaseContext.Currency.Add(ses);

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var ses = _databaseContext.Currency
            .FirstOrDefault(p => p.CurrencyId == id);

            _databaseContext.Currency.Remove(ses);
            _databaseContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var ses = _databaseContext.Currency
            .FirstOrDefault(p => p.CurrencyId == id);

            ViewData["Success"] = TempData["Success"];

            var model = new EditCurrencyViewModel
            {
                Currency = ses
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id, EditCurrencyViewModel model)
        {


            if (ModelState.IsValid)
            {
                var ses = _databaseContext.Currency

                .FirstOrDefault(m => m.CurrencyId == id);

                ses.CurrencyName = model.Currency.CurrencyName;

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
