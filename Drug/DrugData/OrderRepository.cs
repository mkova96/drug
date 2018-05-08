using DrugData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrugData
{
    public class OrderRepository:IOrderRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly Cart _cart;


        public OrderRepository(ApplicationDbContext applicationDbContext, Cart cart)
        {
            _applicationDbContext = applicationDbContext;
            _cart = cart;

        }

        public void CreateOrder(Order order, string userId)
        {
            var user = _applicationDbContext.Users.FirstOrDefault(g => g.Id == userId);
            order.OrderDate = DateTime.Now;
            order.PriceSum = _cart.getCartTotal();
            _applicationDbContext.Order.Add(order);


            var cartItems = _cart.DrugCarts;

            foreach (var item in cartItems)
            {
                var drug = _applicationDbContext.Drug.FirstOrDefault(t => t.DrugId == item.Drug.DrugId);
                var ord = _applicationDbContext.Order.FirstOrDefault(t => t.OrderId ==order.OrderId);

                var orderDetail = new OrderDetail()
                {
                    Amount = item.Quantity,
                    Drug = drug,
                    Order = ord,
                    Price = item.Drug.Price,
                };
                _applicationDbContext.OrderDetail.Add(orderDetail);
            }
            _applicationDbContext.SaveChanges();
        }
    }

}
