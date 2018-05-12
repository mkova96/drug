using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData.Components
{
    public class CartSummary:ViewComponent
    {
        private readonly Cart _cart;
        public CartSummary(Cart cart)
        {
            _cart = cart;
        }
        public IViewComponentResult Invoke()
        {
            var items = _cart.GetCartDrugs();
            _cart.DrugCarts = items;

            var cartViewModel = new CartViewModel
            {
                Cart = _cart,
                CartTotal = _cart.getCartTotal()
            };
            return View(cartViewModel);
        }
    }
}
