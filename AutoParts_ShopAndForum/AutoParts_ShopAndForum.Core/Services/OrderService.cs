using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
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

        public OrderSummaryModel[] GetAllByUserId(string userId)
        {
            var result = new List<OrderSummaryModel>();

            var userOrders = _context.Orders
                .Include(u => u.User)
                .Include(t => t.Town)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.UserId == userId)
                .ToArray();

            foreach (var order in userOrders)
            {
                var currentOrder = new OrderSummaryModel()
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

        public OrderModel PlaceOrderAndClearCart(
            ref ICollection<ProductCartModel> cart, OrderInputModel inputModel)
        {
            var order = new Order();
            Infrastructure.Data.Models.OnlinePaymentStatus? onlinePaymentStatus = null;

            if (inputModel.PayWay == Models.Order.OrderPayWay.OnlinePayment)
            {
                onlinePaymentStatus = Infrastructure.Data.Models.OnlinePaymentStatus.Pending;
            }

            int? courierStatioId = null;

            if (inputModel.DeliveryMethod == Models.Order.DeliveryMethod.PersonalTake)
                courierStatioId = inputModel.CourierStationId;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    order = new Order
                    {
                        PublicToken = Guid.NewGuid(),
                        OverallSum = cart.Sum(p => p.Price * p.Quantity),
                        DateCreated = DateTime.Now,
                        IsDelivered = false,
                        DeliveryMethod = DeliveryMethodToDb(inputModel.DeliveryMethod),
                        PayWay = PayWayToDb(inputModel.PayWay),
                        OnlinePaymentStatus = onlinePaymentStatus,
                        DeliveryStreet = inputModel.DeliveryStreet,
                        TownId = inputModel.TownId,
                        CourierStationId = courierStatioId,
                        UserId = inputModel.UserId,
                        InvoicePersonFirstName = inputModel.InvoicePersonFirstName,
                        InvoicePersonLastName = inputModel.InvoicePersonLastName,
                        InvoiceAddress = inputModel.InvoiceAddress,
                    };

                    order = _context.Orders.Add(order).Entity; // todo double check

                    foreach (var product in cart)
                    {
                        order.OrderProducts.Add(new OrderProduct()
                        {
                            ProductId = product.Id,
                            SinglePrice = product.Price,
                            Quantity = product.Quantity
                        });
                    }

                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch (System.Exception e)
                {
                    transaction.Rollback();

                    throw e;
                }
            }

            return OrderModelProjection(order);
        }

        public OrderModel FindByPublicToken(Guid orderToken)
        {
            return _context.Orders
                .Select(OrderModelProjection)
                .FirstOrDefault(o => o.PublicToken == orderToken);
        }

        public int MarkOnlinePaymentAsSuccessful(Guid orderToken)
        {
            var order = _context.Orders
                .FirstOrDefault(o => o.PublicToken == orderToken);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.PayWay != Infrastructure.Data.Models.OrderPayWay.OnlinePayment)
                throw new InvalidOperationException("Selected order is not registered for online payment.");

            order.OnlinePaymentStatus = Infrastructure.Data.Models.OnlinePaymentStatus.SuccessfullyPaid;
            
            _context.SaveChanges();
            
            return order.Id;
        }

        public int MarkOnlinePaymentAsCancelled(Guid orderToken)
        {
            var order = _context.Orders
                .FirstOrDefault(o => o.PublicToken == orderToken);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.PayWay != Infrastructure.Data.Models.OrderPayWay.OnlinePayment)
                throw new InvalidOperationException("Selected order is not registered for online payment.");

            order.OnlinePaymentStatus = Infrastructure.Data.Models.OnlinePaymentStatus.Cancelled;
            
            _context.SaveChanges();
            
            return order.Id;
        }

        private OrderModel OrderModelProjection(Order order)
        {
            return new OrderModel
            {
                Id = order.Id,
                PublicToken = order.PublicToken,
                // todo and so on
            };
        }

        private Infrastructure.Data.Models.OrderPayWay PayWayToDb(Models.Order.OrderPayWay inputModelPayWay)
        {
            switch (inputModelPayWay)
            {
                case Models.Order.OrderPayWay.CashOnDelivery:
                    return Infrastructure.Data.Models.OrderPayWay.CashOnDelivery;
                case Models.Order.OrderPayWay.OnlinePayment:
                    return Infrastructure.Data.Models.OrderPayWay.OnlinePayment;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputModelPayWay), inputModelPayWay, null);
            }
        }

        private Infrastructure.Data.Models.DeliveryMethod DeliveryMethodToDb(
            Models.Order.DeliveryMethod inputModelDeliveryMethod)
        {
            switch (inputModelDeliveryMethod)
            {
                case Models.Order.DeliveryMethod.DeliverToAddress:
                    return Infrastructure.Data.Models.DeliveryMethod.DeliverToAddress;
                case Models.Order.DeliveryMethod.PersonalTake:
                    return Infrastructure.Data.Models.DeliveryMethod.PersonalTake;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputModelDeliveryMethod), inputModelDeliveryMethod, null);
            }
        }
    }
}
