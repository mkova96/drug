using DrugData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LijekData.Models
{
    public class Cart
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private Cart(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        [Key]
        [Required]
        public Guid CartId { get; set; }

        public ICollection<DrugCart> DrugCarts { get; set; }



        public static Cart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            var context = services.GetService<ApplicationDbContext>();
            String cartID = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartID);

            return new Cart(context) { CartId = Guid.Parse(cartID) };
        }

        public void AddToCart(Drug album, int quantity)
        {
            var cartAlbum = _applicationDbContext.DrugCart.SingleOrDefault(s => s.Drug.DrugId ==album.DrugId && s.Cart.CartId == CartId);
            var cart = _applicationDbContext.Cart.FirstOrDefault(t => t.CartId == CartId);
            if (cartAlbum == null)
            {
                cartAlbum = new DrugCart
                {
                    Cart=cart,//paz
                    Drug = album,
                    Quantity = 1
                };
                _applicationDbContext.DrugCart.Add(cartAlbum);
            }
            else
            {
                cartAlbum.Quantity++;
            }
            _applicationDbContext.SaveChanges();


        }
        public int RemoveFromCart(Drug album)
        {
            var cartAlbum = _applicationDbContext.DrugCart.SingleOrDefault(s => s.Drug.DrugId == album.DrugId && s.Cart.CartId == CartId);

            var localAmount = 0;
            if (cartAlbum != null)
            {
                if (cartAlbum.Quantity > 1)
                {
                    cartAlbum.Quantity--;
                    localAmount = cartAlbum.Quantity;
                }
                else
                {
                    _applicationDbContext.DrugCart.Remove(cartAlbum);
                }

            }

            _applicationDbContext.SaveChanges();
            return localAmount;
        }
        public ICollection<DrugCart> GetCartDrugs()
        {
            return DrugCarts ?? (DrugCarts = _applicationDbContext.DrugCart.Where(c => c.Cart.CartId == CartId).Include(s => s.Drug).ToList());
        }
        public void ClearCart()
        {
            var cartAlbums = _applicationDbContext.DrugCart.Where(cart => cart.Cart.CartId == CartId);

            _applicationDbContext.DrugCart.RemoveRange(cartAlbums);

            _applicationDbContext.SaveChanges();
        }
        public decimal getCartTotal()
        {
            var total = _applicationDbContext.DrugCart.Where(c => c.Cart.CartId == CartId).Select(c => c.Drug.Price * c.Quantity).Sum();
            return total;
        }



    }
}
