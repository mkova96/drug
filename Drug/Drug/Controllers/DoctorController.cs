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
            var students = from s in _databaseContext.Doctor.Include(t => t.City).ThenInclude(t => t.Country)

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
            return View(await PaginatedList<Doctor>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
        }

        public ViewResult List()
        {
            ViewData["Success"] = TempData["Success"];
            IEnumerable<Doctor> docs = _databaseContext.Doctor.Include(t =>t.Specialization).ToList();
            return View(docs);
        }


        [HttpGet]
        public ViewResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Specializations"] = _databaseContext.Specialization.ToList();
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();

            return View(new DoctorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();
            ViewData["Specializations"] = _databaseContext.Specialization.ToList();

            var city = _databaseContext.City.FirstOrDefault(m => m.CityId == model.CityId);
            var country = _databaseContext.Country.FirstOrDefault(m => m.CountryId == 1);

            city.Country = country;

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
                ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();
                ViewData["Specializations"] = _databaseContext.Specialization.ToList();
                return View("Add", model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            var user = _databaseContext.Doctor.FirstOrDefault(g => g.Id == id);
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();
            ViewData["Specializations"] = _databaseContext.Specialization.ToList();
            ViewData["Success"] = TempData["Success"];
            var model = new EditDoctorViewModel
            {
                Doctor = user
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(string id, EditDoctorViewModel model)
        {
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();
            ViewData["Specializations"] = _databaseContext.Specialization.ToList();

            var city = _databaseContext.City.FirstOrDefault(m => m.CityId == model.CityId);
            var country = _databaseContext.Country.FirstOrDefault(m => m.CountryId == 1);

            city.Country = country;

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

                var user = _databaseContext.Doctor.FirstOrDefault(g => g.Id == id);
                user.Email = model.Doctor.Email;
                user.Name = model.Doctor.Name;
                user.Surname = model.Doctor.Surname;
                user.Address = model.Doctor.Address;
                user.City = city;
                user.UserName = model.Doctor.Email;
                user.NormalizedUserName = model.Doctor.Email.ToUpper();
                user.NormalizedEmail = model.Doctor.Email.ToUpper();
                user.Title = model.Doctor.Title;
                user.Biography = model.Doctor.Biography;
                user.UserDate = DateTime.Now;
                user.Specialization = man;

                var x = _databaseContext.User.Where(g => (g.Email == user.Email && g.Id != id)).ToList();
                if (x.Count > 0)
                {
                    TempData[Constants.Message] = $"Korisnik s tim mailom već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Edit", new { id = id });
                }

                TempData["Success"] = true;
                _databaseContext.SaveChanges();
                TempData[Constants.Message] = $"Doktor je promijenjen";
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
            var user = _databaseContext.User.Find(id);

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

            var x = _databaseContext.Doctor.ToList().Count;


            if ((page - 1) * 8 == x - 1 && page != 1)
            {
                --page;
            }


            return RedirectToAction(nameof(Index), new { page = page });
        }
    }
}
