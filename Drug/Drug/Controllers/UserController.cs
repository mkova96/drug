using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class UserController:Controller
    {

        private readonly ApplicationDbContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public UserController(ApplicationDbContext context, UserManager<User> userManager, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _databaseContext = context;
            _userManager = userManager;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public async Task<ViewResult> Index()
        {
            ViewData["Success"] = TempData["Success"];
            var user = await _userManager.GetUserAsync(User);
            IEnumerable<User> users = _databaseContext.Users.ToList().Where(p=>p.IsDoctor==false).Where(r=>r.IsAdmin==false).Where(t=>t.Id!=user.Id.ToString());
            return View(users);
        }

        //[AllowAnonymous]
        public ViewResult Show(string id)
        {
            var user = _databaseContext.Users.Include(t=>t.City).ThenInclude(t=>t.Country).FirstOrDefault(g => g.Id == id);
            return View(user);
        }

        [HttpGet]
        public ViewResult Add()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                _databaseContext.Users.Add(new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.EMail
                    
                });

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }
            else
            {
                return View("Add", model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            var user = _databaseContext.Users.FirstOrDefault(g => g.Id == id);
            ViewData["Success"] = TempData["Success"];
            var model = new EditUserViewModel
            {
                User = user
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(string id, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _databaseContext.Users.FirstOrDefault(g => g.Id == id);
                user.Email = model.User.Email;
                user.Name = model.User.Name;
                user.Surname = model.User.Surname;
                TempData["Success"] = true;
                _databaseContext.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _databaseContext.Users.Find(id);
            _databaseContext.Database.ExecuteSqlCommand("DELETE FROM \"AspNetUsers\" WHERE \"AspNetUsers\".\"Id\" = {0}", id);
            _databaseContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



    }
}
