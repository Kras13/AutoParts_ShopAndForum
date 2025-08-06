using AutoParts_ShopAndForum.Areas.Administrator.Models.Order;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Models.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Administrator.Controllers;

public class OrderController(IOrderService orderService, ITownService townService) : BaseAdminController
{
    public IActionResult Index(OrderQueryViewModel viewModel, bool isOrderUpdated = false)
    {
        var queryModel = orderService.GetQueried(
            viewModel.CurrentPage, viewModel.OrdersPerPage, viewModel.Sorting, viewModel.SelectedStatusFilter);

        viewModel.TotalOrders = queryModel.TotalOrdersWithoutPagination;
        viewModel.Orders = queryModel.Orders
            .Select(o => new OrderSummaryModel
            {
                OrderId = o.Id,
                Username = o.Username,
                DateCreated = o.DateCreated,
                DateDelivered = o.DateDelivered,
                PayWay = o.PayWay,
                OverallSum = o.OverallSum,
            }).ToArray();
        
        ViewBag.IsOrderUpdated = isOrderUpdated;

        return View(viewModel);
    }

    public IActionResult Edit(int orderId)
    {
        var model = LoadData(orderId, new OrderInputViewModel { OrderId = orderId });

        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(OrderInputViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = LoadData(model.OrderId, model);
            
            return View(model);
        }

        orderService.UpdateOrder(new OrderEditModel
        {
            OrderId = model.OrderId,
            IsDelivered = model.IsDelivered,
            DateDelivered = model.DateDelivered,
        });

        return RedirectToAction(nameof(Index), new { isOrderUpdated = true } );
    }

    private OrderInputViewModel LoadData(int orderId, OrderInputViewModel model)
    {
        var orderDetails = orderService.GetOrderDetails(orderId, User.GetId());

        model.DateDelivered = orderDetails.DateDelivered;
        model.IsDelivered = orderDetails.IsDelivered;
        model.ReadOnlyModel = new CheckoutFormModel
        {
            ReadOnlyMode = true,
            DeliveryMethod = orderDetails.DeliveryMethod,
            DeliveryStreet = orderDetails.DeliveryStreet,
            SelectedTownId = orderDetails.TownId,
            SelectedCourierStationId = orderDetails.CourierStationId,
            InvoiceFirstName = orderDetails.InvoiceFirstName,
            InvoiceLastName = orderDetails.InvoiceLastName,
            InvoiceAddress = orderDetails.InvoiceAddress,
            PayWay = orderDetails.PayWay,
            Towns = townService.GetAll(),
            Products = orderDetails.Products
                .Select(p => new ProductCartModel
                {
                    Id = p.Id,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                }).ToArray()
        };

        return model;
    }
}