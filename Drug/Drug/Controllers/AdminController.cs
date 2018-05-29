using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class AdminController:Controller
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;



        public AdminController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager)

        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "Id_desc" : "Id";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var user = await _userManager.GetUserAsync(User);

            var students = from s in _databaseContext.Users.Include(t => t.City).ThenInclude(t => t.Country)
                           .Where(p => p.IsAdmin == true).Where(t => t.Id != user.Id.ToString())
                           .Where(p => p.UserName != "ADMIN").Where(t => t.Id != user.Id.ToString())

            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString) || s.Surname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Surname);
                    break;
                case "Id":
                    students = students.OrderBy(s => s.UserDate);
                    break;
                case "Id_desc":
                    students = students.OrderByDescending(s => s.UserDate);
                    break;
                default:
                    students = students.OrderBy(s => s.Surname);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<User>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        [HttpGet]
        public ViewResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();

            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();

            var city = _databaseContext.City.FirstOrDefault(m => m.CityId == model.CityId);
            var country = _databaseContext.Country.FirstOrDefault(m => m.CountryId == 1);

            city.Country = country;

            if (ModelState.IsValid)
            {
                var doc = new User
                {
                    UserName = model.EMail,
                    Email = model.EMail,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserDate = DateTime.Now,
                    Address = model.Address,
                    City = city,
                    IsDoctor = false,
                    IsAdmin = true
                };

                var x = _databaseContext.User.FirstOrDefault(g => g.Email == doc.Email);
             
                if (x != null)
                {
                    TempData[Constants.Message] = $"Korisnik s tim mailom već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add),new { retUrl=returnUrl });
                }

                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    await _roleManager.CreateAsync(role);
                }

                var result = await _userManager.CreateAsync(doc, model.Password);
                if (result.Succeeded)
                {
                    var currentUser = _userManager.FindByNameAsync(doc.UserName);
                    await _userManager.AddToRoleAsync(doc, "Admin");
                    TempData[Constants.Message] = $"Admin je dodan";
                    TempData[Constants.ErrorOccurred] = false;

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction(nameof(AdminController.Index));
                }
                AddErrors(result);
            }
            else
            {
                ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();
                return View("Add", model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            var user = _databaseContext.Users.FirstOrDefault(g => g.Id == id);
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();

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
            var city = _databaseContext.City.FirstOrDefault(m => m.CityId == model.CityId);
            var country = _databaseContext.Country.FirstOrDefault(m => m.CountryId == 1);

            city.Country = country;

            if (ModelState.IsValid)
                {
                    var user = _databaseContext.Users.FirstOrDefault(g => g.Id == id);
                    user.Email = model.User.Email;
                    user.Name = model.User.Name;
                    user.Surname = model.User.Surname;
                    user.Address = model.User.Address;
                    user.City = city;
                user.UserName = model.User.Email;
                user.NormalizedUserName = model.User.Email.ToUpper();
                user.NormalizedEmail =  model.User.Email.ToUpper();

                var x = _databaseContext.User.Where(g => (g.Email == user.Email && g.Id != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Korisnik s tim mailom već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                    _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Admin je promijenjen";
                TempData[Constants.ErrorOccurred] = false;
            }

                      
            return RedirectToAction(nameof(Index));
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, int? page)
        {
            var user = _databaseContext.Users.Find(id);

            var comments = _databaseContext.Comment.Include(i => i.User).Where(t => t.User.Id == id).ToList();
            foreach (var t in comments)
            {
                _databaseContext.Comment.Remove(t);
            }
            _databaseContext.SaveChanges();

            var messages = _databaseContext.Message.Include(i => i.Sender).Include(t => t.Receiver).Where(t => (t.Sender.Id == id || t.Receiver.Id == id)).ToList();
            foreach (var t in messages)
            {
                _databaseContext.Message.Remove(t);
            }
            _databaseContext.SaveChanges();

            var orders = _databaseContext.Order.Include(i => i.User).Where(t => t.User.Id == id).ToList();
            foreach (var t in orders)
            {
                _databaseContext.Order.Remove(t);
            }
            _databaseContext.SaveChanges();

            _databaseContext.Database.ExecuteSqlCommand("DELETE FROM \"AspNetUsers\" WHERE \"AspNetUsers\".\"Id\" = {0}", id);
            _databaseContext.SaveChanges();

            var userx = await _userManager.GetUserAsync(User);


            var x = _databaseContext.Users.Where(p => p.IsAdmin == true).Where(t => t.Id != userx.Id.ToString()).ToList().Count;


            if ((page - 1) * 8 == x-1 && page != 1)
            {
                --page;
            }


            return RedirectToAction(nameof(Index), new { page = page });
        }
    }
}
