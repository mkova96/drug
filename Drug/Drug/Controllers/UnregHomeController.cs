using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Drug.Controllers
{
    public class UnregHomeController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;

        public UnregHomeController(ApplicationDbContext context)
        {
            _databaseContext = context;
        }

        public ViewResult Index()
        {
            ViewData["Empty"] = "true";
            //ViewData["Users"] = _databaseContext.Users.ToList();
            ViewData["Manufacturers"] = _databaseContext.Manufacturer.ToList();
            return View();
        }

    }
}
