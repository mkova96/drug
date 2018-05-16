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

        public async Task<ViewResult> Index()
        {
            ViewData["Success"] = TempData["Success"];
            var user = await _userManager.GetUserAsync(User);
            IEnumerable<User> docs = _databaseContext.Users.Where(p=>p.IsAdmin==true).Where(t => t.Id != user.Id.ToString()).Where(t => t.Id != user.Id.ToString()).ToList();
            return View(docs);
        }
        public ViewResult Show(string id)
        {
            var user = _databaseContext.Users.Include(t => t.City).ThenInclude(p => p.Country).FirstOrDefault(g => g.Id == id);
            return View(user);
        }

        [HttpGet]
        public ViewResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");


            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");

            City city = _databaseContext.City.FirstOrDefault(c => c.PostCode == Int32.Parse(model.PostCode));
            if (city == null)
            {
                City city1 = new City
                {
                    PostCode = Int32.Parse(model.PostCode),
                    CityName = model.CityName,
                    Country = _databaseContext.Country.FirstOrDefault(c => c.CountryId == model.CountryID)
                };

                _databaseContext.City.Add(city1);


                Country country = _databaseContext.Country.FirstOrDefault(c => c.CountryId == model.CountryID);
                country.Cities.Add(city1);

                _databaseContext.Entry(country).State = EntityState.Modified;
                _databaseContext.SaveChanges();
                city = city1;
            }

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
                ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");
                return View("Add", model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            var user = _databaseContext.Users.FirstOrDefault(g => g.Id == id);
            ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");

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

            City city = _databaseContext.City.FirstOrDefault(c => c.PostCode == model.PostCode);
            if (city == null)
            {
                City city1 = new City
                {
                    PostCode = model.PostCode,
                    CityName = model.User.City.CityName,
                    Country = _databaseContext.Country.FirstOrDefault(c => c.CountryId == model.CountryId)
                };

                _databaseContext.City.Add(city1);


                Country country = _databaseContext.Country.FirstOrDefault(c => c.CountryId == model.CountryId);
                country.Cities.Add(city1);

                _databaseContext.Entry(country).State = EntityState.Modified;
                _databaseContext.SaveChanges();
                city = city1;
            }

                if (ModelState.IsValid)
                {
                    var user = _databaseContext.Users.FirstOrDefault(g => g.Id == id);
                    user.Email = model.User.Email;
                    user.Name = model.User.Name;
                    user.Surname = model.User.Surname;
                    user.Address = model.User.Address;
                    user.City = city;

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
    }
}
