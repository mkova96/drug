using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
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
            //ViewData["Companies"] = _databaseContext.Companies.Include(c => c.City).ToList();
            return View();
        }

    }
}
