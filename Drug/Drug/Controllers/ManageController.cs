using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class ManageController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _databaseContext;
        private readonly UrlEncoder _urlEncoder;
        private readonly ApplicationDbContext _context;



        public ManageController(
          UserManager<User> userManager,
          SignInManager<User> signInManager,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _databaseContext = context;
            _urlEncoder = urlEncoder;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nema korisnika sa zadanim ID-em '{_userManager.GetUserId(User)}'.");
            }
            ViewBag.Items = new SelectList(_context.Country, "Id", "Name");
            City city = _context.City.FirstOrDefault(c => c.CityId ==1);//PAZI CITY.ID 
            Country country = _context.Country.FirstOrDefault(c => c.CountryId ==1);//ista stvar
            var model = new IndexViewModel
            {
                Username = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                Address = user.Address,
                PostCode = city.PostCode,
                City = city.CityName,
                CountryID = country.CountryId,
                Email = user.Email,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            ViewBag.Items = new SelectList(_context.Country, "CountryId", "CountryName");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nema korisnika sa zadanim ID-em '{_userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Neočekivana greška se javila prilikom postavljanja e-mail adrese korisnika s ID-em '{user.Id}'.");
                }
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Neočekivana greška se javila prilikom postavljanja e-mail adrese korisnika s ID-em '{user.Id}'.");
                }
            }

            var firstName = user.Name;
            if (model.Name != firstName)
            {
                User appuser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                appuser.Name = model.Name;
                _context.Entry(appuser).State = EntityState.Modified;
                _context.SaveChanges();
            }

            var lastName = user.Surname;
            if (model.Surname != lastName)
            {
                User appuser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                appuser.Surname = model.Surname;
                _context.Entry(appuser).State = EntityState.Modified;
                _context.SaveChanges();
            }

            var address = user.Address;
            if (model.Address != address)
            {
                User appuser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                appuser.Address = model.Address;
                _context.Entry(appuser).State = EntityState.Modified;
                _context.SaveChanges();
            }
            City cityhelp = _context.City.FirstOrDefault(c => c.CityId == 1);//pazi
            if (model.PostCode != cityhelp.PostCode)
            {
                City city = _context.City.FirstOrDefault(c => c.PostCode == model.PostCode);
                if (city == null)
                {
                    City city1 = new City
                    {
                        PostCode = model.PostCode,
                        CityName = model.City,
                        Country = _context.Country.FirstOrDefault(c => c.CountryId == model.CountryID)
                    };

                    _context.City.Add(city1);


                    Country country = _context.Country.FirstOrDefault(c => c.CountryId == model.CountryID);
                    country.Cities.Add(city1);

                    _context.Entry(country).State = EntityState.Modified;
                    _context.SaveChanges();
                    city = city1;
                }
                User appuser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                appuser.City = city;
                _context.Entry(appuser).State = EntityState.Modified;
                _context.SaveChanges();

            }

                StatusMessage = "Vaš profil je izmijenjen!";
                return RedirectToAction(nameof(Index));
            

        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nema korisnika sa zadanim ID-em '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nema korisnika sa zadanim ID-em '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Vaša lozinka je uspješno promijenjena.";

            return RedirectToAction(nameof(ChangePassword));
        }

        /*[HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nema korisnika sa zadanim ID-em '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Nema korisnika sa zadanim ID-em '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Vaša lozinka je postavljena.";

            return RedirectToAction(nameof(SetPassword));
        }*/

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion







    }
}
