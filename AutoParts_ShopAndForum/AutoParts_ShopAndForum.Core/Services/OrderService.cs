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
        private readonly IOrderNotification _orderNotification;

        public OrderService(ApplicationDbContext context, IOrderNotification orderNotification)
        {
            _context = context;
            _orderNotification = orderNotification;
        }

        public OrderPagedModel GetAllByUserId(string userId, int pageNumber, int pageSize)
        {
            var userOrders = _context.Orders
                .Include(u => u.User)
                .Include(t => t.Town)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .OrderBy(x => x.Id)
                .Where(u => u.UserId == userId)
                .AsQueryable();
            
            var totalOrders = userOrders.Count();
            var orders = userOrders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList()
                .Select(OrderModelProjection)
                .ToArray();
            
            return new OrderPagedModel
            {
                TotalProductsWithoutPagination = totalOrders,
                Orders = orders,
            };
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
            
            var town = _context.Towns.FirstOrDefault(t => t.Id == inputModel.TownId);
            
            if (town == null)
                throw new ArgumentException("Invalid town");
            
            var user = _context.Users.FirstOrDefault(u => u.Id == inputModel.UserId);

            if (user == null)
                throw new ArgumentException("Invalid user");
            
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
                        Town = town,
                        CourierStationId = courierStatioId,
                        User = user,
                        InvoicePersonFirstName = inputModel.InvoicePersonFirstName,
                        InvoicePersonLastName = inputModel.InvoicePersonLastName,
                        InvoiceAddress = inputModel.InvoiceAddress,
                    };

                    order = _context.Orders.Add(order).Entity;

                    foreach (var product in cart)
                    {
                        order.OrderProducts.Add(new OrderProduct()
                        {
                            ProductId = product.Id,
                            SinglePrice = product.Price,
                            Quantity = product.Quantity
                        });
                    }
                    
                    _orderNotification.SendNotification();
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

        public OrderDetailsModel GetOrderDetails(int orderId, string userId)
        {
            var order = _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.Town)
                .Include(o => o.CourierStation)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.UserId != userId)
                throw new ArgumentException("User does not have access to this order.");

            return new OrderDetailsModel
            {
                Id = order.Id,
                Products = order.OrderProducts.Select(p => new OrderProductModel
                {
                    Id = p.ProductId,
                    SinglePrice = p.SinglePrice,
                    Quantity = p.Quantity,
                    ImageUrl = p.Product.ImageUrl,
                }).ToArray(),
                OverallSum = order.OverallSum,
                InvoiceFirstName = order.InvoicePersonFirstName,
                InvoiceLastName = order.InvoicePersonLastName,
                InvoiceAddress = order.InvoiceAddress,
                DeliveryMethod = FromDbDeliveryMethod(order.DeliveryMethod),
                DeliveryStreet = order.DeliveryStreet,
                Town = order.Town.Name,
                CourierStationAddress = order.CourierStation?.FullAddress,
                DateDelivered = order.DateDelivered,
                OnlinePaymentStatus = FromDbOnlinePaymentStatus(order.OnlinePaymentStatus),
            };
        }

        private OrderModel OrderModelProjection(Order order)
        {
            return new OrderModel
            {
                Id = order.Id,
                PublicToken = order.PublicToken,
                DateCreated = order.DateCreated,
                IsDelivered = order.IsDelivered,
                OverallSum = order.OverallSum,
                DeliveryMethod = FromDbDeliveryMethod(order.DeliveryMethod),
                PayWay = FromDbPayWay(order.PayWay),
                OnlinePaymentStatus = FromDbOnlinePaymentStatus(order.OnlinePaymentStatus),
                DateDelivered = order.DateDelivered,
                DeliveryStreet = order.DeliveryStreet,
                Town = order.Town?.Name,
            };
        }

        private Core.Models.Order.OnlinePaymentStatus? FromDbOnlinePaymentStatus(
            Infrastructure.Data.Models.OnlinePaymentStatus? onlinePaymentStatus)
        {
            if (!onlinePaymentStatus.HasValue)
                return null;
            
            switch (onlinePaymentStatus)
            {
                case Infrastructure.Data.Models.OnlinePaymentStatus.Pending:
                    return Models.Order.OnlinePaymentStatus.Pending;
                case Infrastructure.Data.Models.OnlinePaymentStatus.Cancelled:
                    return Models.Order.OnlinePaymentStatus.Cancelled;
                case Infrastructure.Data.Models.OnlinePaymentStatus.SuccessfullyPaid:
                    return Models.Order.OnlinePaymentStatus.SuccessfullyPaid;
                default:
                    throw new ArgumentOutOfRangeException(nameof(onlinePaymentStatus), onlinePaymentStatus, null);
            }
        }
        
        private Core.Models.Order.DeliveryMethod FromDbDeliveryMethod(
            Infrastructure.Data.Models.DeliveryMethod deliveryMethod)
        {
            switch (deliveryMethod)
            {
                case Infrastructure.Data.Models.DeliveryMethod.DeliverToAddress:
                    return Models.Order.DeliveryMethod.DeliverToAddress;
                case Infrastructure.Data.Models.DeliveryMethod.PersonalTake:
                    return Models.Order.DeliveryMethod.PersonalTake;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deliveryMethod), deliveryMethod, null);
            }
        }

        private Core.Models.Order.OrderPayWay FromDbPayWay(Infrastructure.Data.Models.OrderPayWay payWay)
        {
            switch (payWay)
            {
                case Infrastructure.Data.Models.OrderPayWay.CashOnDelivery:
                    return Models.Order.OrderPayWay.CashOnDelivery;
                case Infrastructure.Data.Models.OrderPayWay.OnlinePayment:
                    return Models.Order.OrderPayWay.OnlinePayment;
                default:
                    throw new ArgumentOutOfRangeException(nameof(payWay), payWay, null);
            }
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
