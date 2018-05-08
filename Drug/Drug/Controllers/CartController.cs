using DrugData;
using DrugData.Models;
using DrugData.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lijek.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Cart _cart;
        public CartController(Cart cart, ApplicationDbContext context)
        {
            _cart = cart;
            _context = context;
        }

        public ViewResult Index()
        {
            System.Diagnostics.Debug.WriteLine("INDEXXX");

            var items = _cart.GetCartDrugs();
            System.Diagnostics.Debug.WriteLine("broj" + items.Count);
            _cart.DrugCarts = items;

            var cVM = new CartViewModel
            {
                Cart = _cart,
                CartTotal = _cart.getCartTotal()

            };
            return View(cVM);
        }
        public RedirectToActionResult AddToCart(int drugId)
        {

            var selected = _context.Drug.FirstOrDefault(p => p.DrugId == drugId);
            System.Diagnostics.Debug.WriteLine("Dodan" + selected.DrugName.ToString());

            if (selected != null)
            {
                System.Diagnostics.Debug.WriteLine("Dodan" + selected.DrugName.ToString());
                _cart.AddToCart(selected, 1);
            }
            return RedirectToAction("Index");
        }
        public RedirectToActionResult RemoveFromCart(int drugId)
        {
            var selected = _context.Drug.FirstOrDefault(p => p.DrugId == drugId);
            if (selected != null)
            {
                _cart.RemoveFromCart(selected);
            }
            return RedirectToAction("Index");
        }
    }

}
