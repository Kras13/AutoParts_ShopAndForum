using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoParts_ShopAndForum.Areas.Identity.Pages.Account.Manage;

public class OrdersModel(IOrderService orderService) : PageModel
{
    public OrderPagedModel OrderPagedModel { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    
    public int TotalPages { get; set; }
    
    private const int PageSize = 6;
    
    public void OnGet()
    {
        OrderPagedModel = orderService.GetAllByUserId(User.GetId(), CurrentPage, PageSize);
        TotalPages = (int)Math.Ceiling(OrderPagedModel.TotalProductsWithoutPagination / (double)PageSize);
    }
}