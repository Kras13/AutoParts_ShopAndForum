using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public OrderModel[] GetAllByUserId(string userId)
        {
            var result = new List<OrderModel>();

            var userOrders = _context.Orders
                .Include(u => u.User)
                .Include(t => t.Town)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.UserId == userId)
                .ToArray();

            foreach (var order in userOrders)
            {
                var currentOrder = new OrderModel()
                {
                    Id = order.Id,
                    Street = order.DeliveryStreet,
                    Town = order.Town.Name,
                    Products = order.OrderProducts.Select(p => new ProductCartModel()
                    {
                        Id = p.ProductId,
                        Description = p.Product.Description,
                        ImageUrl = p.Product.ImageUrl,
                        Name = p.Product.Name,
                        Price = p.SinglePrice,
                        Quantity = p.Quantity
                    }).ToArray()
                };

                result.Add(currentOrder);
            }

            return result.ToArray();
        }
    }
}
