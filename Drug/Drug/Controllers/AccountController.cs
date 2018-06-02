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
        private readonly Cart _cart;


        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            ApplicationDbContext databaseContext,
            RoleManager<IdentityRole> roleManager,
            Cart cart)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _databaseContext = databaseContext;
            _roleManager = roleManager;
            _cart = cart;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.OrderBy(p=>p.CityPbr).ToList();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Cities"] = _databaseContext.City.OrderBy(p => p.CityPbr).ToList();

            if (!await _roleManager.RoleExistsAsync("Owner"))
            {
                var role = new IdentityRole("Owner");
                await _roleManager.CreateAsync(role);
                var admin = new User
                {
                    Name = "Mario",
                    Surname = "Kovačević",
                    Address = "Hipokratova 99",
                    UserName = "sanitas@ljekarna.com",
                    Email = "sanitas@ljekarna.com",
                    City = _databaseContext.City.FirstOrDefault(c => c.CityId==1),
                    IsAdmin=false,
                    IsDoctor=false

                };
                var result1 = await _userManager.CreateAsync(admin, "sanitas500");
                await _userManager.AddToRoleAsync(admin, "Owner");
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole("User");
                await _roleManager.CreateAsync(role);
            }

            var city = _databaseContext.City.FirstOrDefault(m => m.CityId == model.CityId);
            var country = _databaseContext.Country.FirstOrDefault(m => m.CountryId == 1);

            city.Country = country;

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
            _cart.ClearCart();

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "UnregHome");
        }

    }
}
