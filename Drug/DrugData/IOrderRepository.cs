using DrugData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrugData
{
    public interface IOrderRepository
    {
        void CreateOrder(Order order, string userId);
    }

}