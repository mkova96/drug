using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Lijek.Controllers
{
    [Authorize]
    public class AccountController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _databaseContext;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            ApplicationDbContext databaseContext,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _databaseContext = databaseContext;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.ToList();
            ViewData["Countries"] = _databaseContext.Country.ToList();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.ToList();
            ViewData["Countries"] = _databaseContext.Country.ToList();

            ViewBag.Items = new SelectList(_databaseContext.Country, "CountryId", "CountryName");

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole("Admin");
                await _roleManager.CreateAsync(role);
                var admin = new User
                {
                    Name = "Mario",
                    Surname = "Kovačević",
                    Address = "Hipokratova 99",
                    UserName = "ADMIN",
                    Email = "admin@lijekovi.com",
                    City = _databaseContext.City.FirstOrDefault(c => c.CityName == "Zagreb"),
                    IsAdmin=true,
                    IsDoctor=false

                };
                var result1 = await _userManager.CreateAsync(admin, "lupocet500");
                await _userManager.AddToRoleAsync(admin, "Admin");
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole("User");
                await _roleManager.CreateAsync(role);
            }

            var city = _databaseContext.City.FirstOrDefault(m => m.CityId == model.CityId);
            var country = _databaseContext.Country.FirstOrDefault(m => m.CountryId == model.CountryID);

            city.Country = country;




            /*City city = _databaseContext.City.FirstOrDefault(c => c.PostCode == Int32.Parse(model.PostCode));
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
            }*/




            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserDate=DateTime.Now,
                    Address = model.Address,
                    City = city,
                    IsDoctor=false,
                    IsAdmin=false
          
                };

                var x = _databaseContext.User.FirstOrDefault(g => g.Email == model.Email);

                if (x != null)
                {
                    TempData[Constants.Message] = $"Korisnik s tim mailom već postoji.\n";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction(nameof(Register), new { retUrl = returnUrl });
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");

                    TempData[Constants.Message] = $"Uspješno ste se registrirali";
                    TempData[Constants.ErrorOccurred] = false;
                    return RedirectToAction("Index", "Drugs");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Index", "Drugs"); ;
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "UnregHome");
        }

    }
}
