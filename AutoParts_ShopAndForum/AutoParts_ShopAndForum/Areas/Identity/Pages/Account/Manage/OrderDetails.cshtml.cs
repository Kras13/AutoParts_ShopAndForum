using AutoParts_ShopAndForum.Core.Contracts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoParts_ShopAndForum.Areas.Identity.Pages.Account.Manage;

public class OrderDetailsModel(IOrderService orderService) : PageModel
{
    //public OrderDetailsModel Orders { get; set; }
    
    public void OnGet()
    {
        
    }
}