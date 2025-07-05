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
        var order = orderService.FindByPublicToken(orderToken);

        if (order == null)
            throw new ArgumentException("Order not found");

        //var domain = $"{Request.Scheme}://{Request.Host}";
        var successUrl = Url.Action("Success", "Checkout", new { orderToken = order.PublicToken }, Request.Scheme);
        var cancelUrl = Url.Action("Cancel", "Checkout", new { orderToken = order.PublicToken }, Request.Scheme);

        var options = new SessionCreateOptions
        {
            Metadata = new()
            {
                { "orderToken", order.PublicToken.ToString() }
            },
            LineItems = new()
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "bgn",
                        UnitAmount = 2599,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Brake Pads",
                            Description = "Fits Honda Accord 2.4 (2007)"
                        }
                    },
                    Quantity = 1
                },
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
        };

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