using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoParts_ShopAndForum.Areas.Identity.Pages.Account.Manage;

public class OrdersModel(IOrderService orderService) : PageModel
{
    public OrderSummaryModel[] Orders { get; set; }
    
    public void OnGetAsync()
    {
        Orders = orderService.GetAllByUserId(User.GetId());
    }
}