using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Models.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Components;

public class CheckoutItemsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>("Cart");

        if (cart == null || cart.Count == 0)
        {
            throw new ArgumentException("Cart is empty");
        }

        var total = cart.Sum(x => x.Total);
        var transportValue = -1; // todo this must be updated

        var model = new CheckoutItemsModel
        {
            Products = cart,
            Total = total,
            TransportValue = transportValue,
        };

        return View("Default",model); 
    }
}
