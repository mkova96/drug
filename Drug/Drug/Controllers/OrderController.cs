
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

        public OrderController(IOrderRepository orderRepository, Cart cart, UserManager<User> userManager)
        {
            _orderRepository = orderRepository;
            _cart = cart;
            _userManager = userManager;
        }
        [Authorize]
        public IActionResult Checkout()
        {
            var items = _cart.GetCartDrugs();
            _cart.DrugCarts = items;
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
                TempData[Constants.Message] = $"Hvala vam na kupnji.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction("Index", "Drugs");
            }

            return View(order);
        }

    }
}
