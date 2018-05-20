﻿using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drug.Controllers
{
    public class ManufacturersController:Controller
    {

        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly UserManager<User> _userManager;

        public ManufacturersController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, UserManager<User> userManager)
        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _userManager = userManager;
        }

        public ViewResult Index(string category)
        {
            string _category = category;
            IEnumerable<Manufacturer> drinks;
            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(category))
            {
                drinks = _databaseContext.Manufacturer.OrderBy(p => p.ManufacturerId).Include(t=>t.Drugs).Where(t=>t.Drugs.Count>0).ToList();
                currentCategory = "Svi proizvođači";
            }
            else
            {
                if (string.Equals("Po broju proizvoda silazno", _category, StringComparison.OrdinalIgnoreCase))
                    drinks = _databaseContext.Manufacturer.Include(t => t.Drugs).Where(t => t.Drugs.Count > 0).OrderByDescending(p => p.Drugs.Count).ToList();
                else 
                    drinks = _databaseContext.Manufacturer.Include(t => t.Drugs).Where(t => t.Drugs.Count > 0).OrderBy(p => p.Drugs.Count).ToList();

                currentCategory = _category;
            }

            return View(new ManufacturersListViewModel
            {
                Manufacturers = drinks,
                CurrentCategory = currentCategory
            });
        }

    }
}
