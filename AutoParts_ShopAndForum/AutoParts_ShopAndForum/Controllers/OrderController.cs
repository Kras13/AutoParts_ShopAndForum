using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        public IActionResult List()
        {
            var userOrders = this._orderService.GetAllByUserId(this.User.GetId());

            return View(userOrders);
        }
    }
}
