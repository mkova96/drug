using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    public class DoctorController:Controller
    {

        private readonly ApplicationDbContext _databaseContext;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;



        public DoctorController(ApplicationDbContext context,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager)

        {
            _databaseContext = context;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ViewResult Index()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Doctor> docs = _databaseContext.Doctor.Include(t=>t.City).ThenInclude(p=>p.Country).ToList();
            return View(docs);
        }

        public ViewResult List()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Doctor> docs = _databaseContext.Doctor.Include(t =>t.Specialization).ToList();
            return View(docs);
        }
        public ViewResult Show(string id)
        {
            var user = _databaseContext.Doctor.Include(i=>i.Specialization).Include(t => t.City).ThenInclude(p => p.Country).FirstOrDefault(g => g.Id == id);
            return View(user);
        }

        [HttpGet]
        public ViewResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Specializations"] = _databaseContext.Specialization.ToList();
            ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");


            return View(new DoctorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");
            ViewData["Specializations"] = _databaseContext.Specialization.ToList();

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
                Specialization man;

                if (model.SpecializationType == "new")
                {
                    // Additional validation before creating the Company
                    var requiredFields = new[]
                    {
                        new Tuple<string, object>("Name", model.Specialization.SpecializationName)

                    };

                    foreach (var field in requiredFields)
                    {
                        if (field.Item2 == null || field.Item2.Equals(""))
                        {
                            ModelState.AddModelError(string.Empty, $"{field.Item1} field is required.");
                        }
                    }
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    man = model.Specialization;
                    _databaseContext.Specialization.Add(man);
                }
                else
                {
                    man = _databaseContext.Specialization.Find(model.SpecializationId);
                }


                //var spec = _databaseContext.Specializations.FirstOrDefault(m => m.Id == model.SpecializationId);
                var doc=new Doctor
                {
                    UserName = model.EMail,
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.EMail,
                    Title = model.Title,
                    Biography = model.About,
                    UserDate = DateTime.Now,
                    Specialization = man,
                    Address = model.Address,
                    City = city,
                    Education = model.Education,
                    IsDoctor=true,
                    IsAdmin=false
                };
                var x = _databaseContext.User.FirstOrDefault(g => g.Email == doc.Email);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Korisnik s tim mailom već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Add), new { retUrl = returnUrl });
                }

                if (!await _roleManager.RoleExistsAsync("Doctor"))
                {
                    var role = new IdentityRole("Doctor");
                    await _roleManager.CreateAsync(role);
                }

                var result = await _userManager.CreateAsync(doc, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(doc, "Doctor");
                    TempData[Constants.Message] = $"Doktor je dodan";
                    TempData[Constants.ErrorOccurred] = false;

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(DoctorController.Index));
                }
                AddErrors(result);
            }
            else
            {
                ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");
                ViewData["Specializations"] = _databaseContext.Specialization.ToList();
                return View("Add", model);
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
