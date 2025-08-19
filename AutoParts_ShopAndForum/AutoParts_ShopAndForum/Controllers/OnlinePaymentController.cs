using AutoParts_ShopAndForum.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace AutoParts_ShopAndForum.Controllers;

[Authorize]
public class OnlinePaymentController(IOrderService orderService) : Controller
{
    public IActionResult Index(Guid orderToken)
    {
        return View(orderToken);
    }

    [HttpPost]
    public IActionResult CreateStripeSession(Guid orderToken)
    {
        var successUrl = Url.Action("Success", "OnlinePayment", new { orderToken = orderToken }, Request.Scheme);
        var cancelUrl = Url.Action("Cancel", "OnlinePayment", new { orderToken = orderToken }, Request.Scheme);
        var options = orderService.CreateStripeSession(successUrl, cancelUrl, orderToken);

        var service = new SessionService();
        var session = service.Create(options);

        return Redirect(session.Url);
    }
    
    public IActionResult Success(Guid orderToken)
    {
        var orderId = orderService.MarkOnlinePaymentAsSuccessful(orderToken);
        
        return RedirectToAction("Success", "Checkout", new { orderId = orderId });
    }

    public IActionResult Cancel(Guid orderToken)
    {
        orderService.MarkOnlinePaymentAsCancelled(orderToken);
        
        return RedirectToAction("Cancel", "Checkout");
    }
}