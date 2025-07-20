using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoParts_ShopAndForum.Areas.Identity.Pages.Account.Manage;

public class OrderDetailsModel(IOrderService orderService) : PageModel
{
    public Core.Models.Order.OrderDetailsModel Order { get; set; }
    
    public IActionResult OnGet(int id)
    {
        var order = orderService.GetOrderDetails(id, User.GetId());

        Order = order;

        return Page();
    }
}