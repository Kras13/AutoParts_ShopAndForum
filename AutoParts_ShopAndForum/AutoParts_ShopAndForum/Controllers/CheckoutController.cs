using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>("Cart");
        
        if (cart == null || cart.Count == 0)
        {
            throw new ArgumentException("Cart is empty");
        }

        return View(cart);
    }
}