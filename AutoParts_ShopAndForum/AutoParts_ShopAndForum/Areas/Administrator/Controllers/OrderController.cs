using AutoParts_ShopAndForum.Areas.Administrator.Models.Order;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Order;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Administrator.Controllers;

public class OrderController(IOrderService orderService) : BaseAdminController
{
    public IActionResult Index(OrderQueryViewModel viewModel)
    {
        var queryModel = orderService.GetQueried(
            viewModel.CurrentPage, viewModel.OrdersPerPage, viewModel.Sorting, viewModel.StatusFilter);

        viewModel.Orders = queryModel.Orders
            .Select(o => new OrderSummaryModel
            {
                OrderId = o.Id,
                Username = o.Username,
                DateCreated = o.DateCreated,
                DateDelivered = o.DateDelivered,
                OnlinePaymentStatus = o.OnlinePaymentStatus,
            }).ToArray();

        return View(viewModel);
    }
}