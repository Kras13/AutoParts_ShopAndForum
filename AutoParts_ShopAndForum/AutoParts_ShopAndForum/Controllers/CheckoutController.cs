using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Core.Models.Order;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Models.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers;

[Authorize]
public class CheckoutController(
    ICourierStationService courierStationService,
    ITownService townService,
    IOrderService orderService)
    : Controller
{
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>("Cart");

        if (cart == null || cart.Count == 0)
        {
            throw new ArgumentException("Cart is empty");
        }

        var towns = townService.GetAll();
        var selectedTownId = towns.FirstOrDefault()?.Id ?? -1;

        var model = new CheckoutFormModel
        {
            Products = cart,
            Towns = towns,
            SelectedTownId = selectedTownId,
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(CheckoutFormModel formModel)
    {
        var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>("Cart");

        var order = orderService.PlaceOrderAndClearCart(
            ref cart, new OrderInputModel
            {
                DeliveryMethod = formModel.DeliveryMethod,
                PayWay = formModel.PayWay,
                DeliveryStreet = formModel.DeliveryStreet,
                TownId = formModel.SelectedTownId,
                CourierStationId = formModel.SelectedCourierStationId,
                InvoicePersonFirstName = formModel.InvoiceFirstName,
                InvoicePersonLastName = formModel.InvoiceLastName,
                InvoiceAddress = formModel.InvoiceAddress,
                UserId = User.GetId(),
            });

        if (formModel.PayWay == OrderPayWay.OnlinePayment)
        {
            return RedirectToAction(
                "Index", "OnlinePayment", new { orderToken = order.PublicToken });
        }

        if (!ModelState.IsValid)
        {
            return View(formModel);
        }
        
        return RedirectToAction(nameof(Success));
    }

    public IActionResult Success(int orderId)
    {
        var model = new CheckoutSuccessModel
        {
            OrderId = orderId,
            UserEmailAddress = User.GetEmail(),
        };
        
        return View(model);
    }
    
    public IActionResult Cancel()
    {
        return View();
    }

    public ICollection<CourierStationModel> GetCourierStationsForTown([FromRoute] int id)
    {
        return courierStationService.GetAllByTownId(id);
    }
}