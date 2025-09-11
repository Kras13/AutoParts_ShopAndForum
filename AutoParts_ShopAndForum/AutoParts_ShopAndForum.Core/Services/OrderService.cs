
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.EmailNotification;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure.Data;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;


namespace AutoParts_ShopAndForum.Core.Services
{
    public class OrderService(
        ApplicationDbContext context,
        IOrderNotificationService orderNotificationService,
        UserManager<User> userManager)
        : IOrderService
    {
        public OrderPagedModel GetAllByUserId(string userId, int pageNumber, int pageSize)
        {
            var userOrders = context.Orders
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

            var town = context.Towns.FirstOrDefault(t => t.Id == inputModel.TownId);

            if (town == null)
                throw new ArgumentException("Invalid town");

            var user = context.Users.FirstOrDefault(u => u.Id == inputModel.UserId);

            if (user == null)
                throw new ArgumentException("Invalid user");

            using (var transaction = context.Database.BeginTransaction())
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

                    order = context.Orders.Add(order).Entity;

                    foreach (var product in cart)
                    {
                        order.OrderProducts.Add(new OrderProduct()
                        {
                            ProductId = product.Id,
                            SinglePrice = product.Price,
                            Quantity = product.Quantity
                        });
                    }

                    var receiverEmail = user.UserName;
                    var receiverFirstName = user.FirstName;

                    context.SaveChanges();

                    transaction.Commit();

                    SendNotificationEmail(receiverEmail, receiverFirstName, order.Id);
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    throw;
                }
            }

            return OrderModelProjection(order);
        }

        public OrderModel FindByPublicToken(Guid orderToken)
        {
            return context.Orders
                .Include(o => o.Town)
                .Include(o => o.User)
                .Select(OrderModelProjection)
                .FirstOrDefault(o => o.PublicToken == orderToken);
        }

        public int MarkOnlinePaymentAsSuccessful(Guid orderToken)
        {
            var order = context.Orders
                .FirstOrDefault(o => o.PublicToken == orderToken);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.PayWay != Infrastructure.Data.Models.OrderPayWay.OnlinePayment)
                throw new InvalidOperationException("Selected order is not registered for online payment.");

            order.OnlinePaymentStatus = Infrastructure.Data.Models.OnlinePaymentStatus.SuccessfullyPaid;

            context.SaveChanges();

            return order.Id;
        }

        public int MarkOnlinePaymentAsCancelled(Guid orderToken)
        {
            var order = context.Orders
                .FirstOrDefault(o => o.PublicToken == orderToken);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.PayWay != Infrastructure.Data.Models.OrderPayWay.OnlinePayment)
                throw new InvalidOperationException("Selected order is not registered for online payment.");

            order.OnlinePaymentStatus = Infrastructure.Data.Models.OnlinePaymentStatus.Cancelled;

            context.SaveChanges();

            return order.Id;
        }

        public OrderDetailsModel GetOrderDetails(int orderId, string userId)
        {
            var order = context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.Town)
                .Include(o => o.CourierStation)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            var user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                throw new ArgumentException("User does not have access to this order.");

            var roles = userManager.GetRolesAsync(user).Result;

            if (order.UserId != userId && !roles.Contains(RoleType.Administrator))
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
                    Description = p.Product.Description,
                    Price = p.Product.Price,
                    Name = p.Product.Name,
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
                TownId = order.TownId,
                CourierStationId = order.CourierStationId,
                IsDelivered = order.IsDelivered,
            };
        }

        public OrderQueryModel GetQueried(
            int currentPage, int ordersPerPage, OrdersSorting sorting, OrderStatusFilter statusFilter)
        {
            var entities = context.Orders
                .Include(t => t.Town)
                .Include(t => t.User)
                .Select(OrderModelProjection)
                .AsQueryable();

            var skipOrdersIndex = ordersPerPage;
            var entitiesToTake = ordersPerPage;
            var totalEntities = entities.Count();

            if (ordersPerPage == IOrderService.AllOrders)
            {
                skipOrdersIndex = 0;
                entitiesToTake = totalEntities;
            }

            entities = sorting switch
            {
                OrdersSorting.NoSorting => entities.OrderBy(o => o.Id),
                OrdersSorting.OverallSumAscending => entities.OrderBy(o => o.OverallSum),
                OrdersSorting.OverallSumDescending => entities.OrderByDescending(o => o.OverallSum),
                OrdersSorting.DateCreatedAscending => entities.OrderBy(o => o.DateCreated),
                OrdersSorting.DateCreatedDescending => entities.OrderByDescending(o => o.DateCreated),
                OrdersSorting.DateDeliveredAscending => entities.OrderBy(o => o.DateDelivered),
                OrdersSorting.DateDeliveredDescending => entities.OrderByDescending(o => o.DateDelivered),
                _ => throw new ArgumentOutOfRangeException(nameof(sorting), sorting, null)
            };

            entities = statusFilter switch
            {
                OrderStatusFilter.All => entities,
                OrderStatusFilter.Pending => entities.Where(e => !e.DateDelivered.HasValue),
                OrderStatusFilter.Delivered => entities.Where(e => e.DateDelivered.HasValue),
                _ => throw new ArgumentOutOfRangeException(nameof(statusFilter), statusFilter, null)
            };

            return new OrderQueryModel
            {
                TotalOrdersWithoutPagination = totalEntities,
                Orders = entities
                    .Skip((currentPage - 1) * skipOrdersIndex)
                    .Take(entitiesToTake)
                    .ToArray(),
            };
        }

        public OrderModel UpdateOrder(OrderEditModel orderEditModel)
        {
            var order = context.Orders
                .Include(o => o.Town)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == orderEditModel.OrderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            order.IsDelivered = orderEditModel.IsDelivered;
            order.DateDelivered = orderEditModel.DateDelivered;

            context.SaveChanges();

            return OrderModelProjection(order);
        }

        private void SendNotificationEmail(string receiverEmail, string receiverFirstName, int id)
        {
            var order = context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                throw new ArgumentException("Order with such id was not found.");

            var orderModel = new OrderEmailModel
            {
                OrderId = order.Id,
                CustomerName = order.InvoicePersonFirstName,
                OrderDate = order.DateCreated,      
                Items = order.OrderProducts.Select(op => new OrderItemEmailModel
                {
                    Name = op.Product.Name,
                    Quantity = op.Quantity,
                    Price = op.SinglePrice
                }).ToList(),
                TotalPrice = order.OverallSum,
            };

            orderNotificationService.SendNotificationAsync(receiverEmail, receiverFirstName, orderModel);
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
                Username = order.User?.UserName
            };
        }

        private Core.Models.Order.OnlinePaymentStatus? FromDbOnlinePaymentStatus(
            Infrastructure.Data.Models.OnlinePaymentStatus? onlinePaymentStatus)
        {
            if (!onlinePaymentStatus.HasValue)
                return null;

            return onlinePaymentStatus switch
            {
                Infrastructure.Data.Models.OnlinePaymentStatus.Pending => Models.Order.OnlinePaymentStatus.Pending,
                Infrastructure.Data.Models.OnlinePaymentStatus.Cancelled => Models.Order.OnlinePaymentStatus.Cancelled,
                Infrastructure.Data.Models.OnlinePaymentStatus.SuccessfullyPaid => Models.Order.OnlinePaymentStatus
                    .SuccessfullyPaid,
                _ => throw new ArgumentOutOfRangeException(nameof(onlinePaymentStatus), onlinePaymentStatus, null)
            };
        }

        private Core.Models.Order.DeliveryMethod FromDbDeliveryMethod(
            Infrastructure.Data.Models.DeliveryMethod deliveryMethod)
        {
            return deliveryMethod switch
            {
                Infrastructure.Data.Models.DeliveryMethod.DeliverToAddress => Models.Order.DeliveryMethod
                    .DeliverToAddress,
                Infrastructure.Data.Models.DeliveryMethod.PersonalTake => Models.Order.DeliveryMethod.PersonalTake,
                _ => throw new ArgumentOutOfRangeException(nameof(deliveryMethod), deliveryMethod, null)
            };
        }

        private Core.Models.Order.OrderPayWay FromDbPayWay(Infrastructure.Data.Models.OrderPayWay payWay)
        {
            return payWay switch
            {
                Infrastructure.Data.Models.OrderPayWay.CashOnDelivery => Models.Order.OrderPayWay.CashOnDelivery,
                Infrastructure.Data.Models.OrderPayWay.OnlinePayment => Models.Order.OrderPayWay.OnlinePayment,
                _ => throw new ArgumentOutOfRangeException(nameof(payWay), payWay, null)
            };
        }

        private Infrastructure.Data.Models.OrderPayWay PayWayToDb(Models.Order.OrderPayWay inputModelPayWay)
        {
            return inputModelPayWay switch
            {
                Models.Order.OrderPayWay.CashOnDelivery => Infrastructure.Data.Models.OrderPayWay.CashOnDelivery,
                Models.Order.OrderPayWay.OnlinePayment => Infrastructure.Data.Models.OrderPayWay.OnlinePayment,
                _ => throw new ArgumentOutOfRangeException(nameof(inputModelPayWay), inputModelPayWay, null)
            };
        }

        private Infrastructure.Data.Models.DeliveryMethod DeliveryMethodToDb(
            Models.Order.DeliveryMethod inputModelDeliveryMethod)
        {
            return inputModelDeliveryMethod switch
            {
                Models.Order.DeliveryMethod.DeliverToAddress => Infrastructure.Data.Models.DeliveryMethod
                    .DeliverToAddress,
                Models.Order.DeliveryMethod.PersonalTake => Infrastructure.Data.Models.DeliveryMethod.PersonalTake,
                _ => throw new ArgumentOutOfRangeException(nameof(inputModelDeliveryMethod), inputModelDeliveryMethod,
                    null)
            };
        }

        public SessionCreateOptions CreateStripeSession(string successUrl, string cancelUrl, Guid orderPublicToken)
        {
            var order = context.Orders
                .FirstOrDefault(x => x.PublicToken == orderPublicToken);

            if (order == null)
                throw new ArgumentException("Order not found");

            var moneyInStots = (long)order.OverallSum * 100;

            var options = new SessionCreateOptions
            {
                Metadata = new()
            {
                { "orderToken", order.PublicToken.ToString() }
            },
                LineItems = new()
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "bgn",
                        UnitAmount = moneyInStots,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Онлайн авточасти",
                            Description = "Онлайн авточасти"
                        }
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            return options;
        }
    }
}