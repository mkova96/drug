
using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Cart _cart;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;


        public OrderController(IOrderRepository orderRepository, Cart cart, UserManager<User> userManager, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _cart = cart;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);

            var userx = _context.User.Include(t => t.City).FirstOrDefault(t => t.Id == user.Id);

            var items = _cart.GetCartDrugs();
            _cart.DrugCarts = items;
            foreach (var x in _cart.DrugCarts)
            {
                if (x.Quantity > x.Drug.Quantity)
                {
                    TempData[Constants.Message] = $"Možete uzeti maksimalno {x.Drug.Quantity} primjerka proizvoda {x.Drug.DrugName}";
                    TempData[Constants.ErrorOccurred] = true;
                    return RedirectToAction("Index", "Cart");
                }
            }
            if (_cart.DrugCarts.Count == 0)
            {
                TempData[Constants.Message] = $"Vaša košarica je prazna.";
                TempData[Constants.ErrorOccurred] = true;
                return RedirectToAction("Index", "Cart");

            }
            return View(new OrderViewModel{ Address=userx.Address,CityName=userx.City.CityName,Email=userx.Email,FirstName=userx.Name,LastName=userx.Surname,ZipCode=userx.City.PostCode.ToString() });
        }

        [HttpPost]
        //[Authorize]
        public IActionResult Checkout(OrderViewModel model)
        {
            System.Diagnostics.Debug.WriteLine("mee"+model.Address);

            var items = _cart.GetCartDrugs();
            _cart.DrugCarts = items;

            
            if (_cart.DrugCarts.Count == 0)
            {
                ModelState.AddModelError("", "Vaša košarica je prazna!");
            }
            

            if (ModelState.IsValid)
            {
                var user = _context.User.FirstOrDefault(t => t.Email == model.Email);
                var order = new Order { Address= model.Address, FirstName = model.FirstName,LastName=model.LastName,User=user,
                    CityName =model.CityName,CountryName="Hrvatska",Email=model.Email,ZipCode=model.ZipCode };

                _orderRepository.CreateOrder(order, _userManager.GetUserId(User));
                _cart.ClearCart();

                foreach(var x in _cart.DrugCarts)
                {
                    x.Drug.Quantity -= x.Quantity;
                }
                _context.SaveChanges();
                

                TempData[Constants.Message] = $"Hvala vam na kupnji.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction("Index", "Drugs");
            }

            return View(model);
        }

    }
}
