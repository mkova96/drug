
using DrugData;
using DrugData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize]
        public IActionResult Checkout()
        {
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
            return View();
        }

        [HttpPost]
        //[Authorize]
        public IActionResult Checkout(Order order)
        {
            var items = _cart.GetCartDrugs();
            _cart.DrugCarts = items;

            
            if (_cart.DrugCarts.Count == 0)
            {
                ModelState.AddModelError("", "Vaša košarica je prazna!");
            }
            

            if (ModelState.IsValid)
            {
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

            return View(order);
        }

    }
}
