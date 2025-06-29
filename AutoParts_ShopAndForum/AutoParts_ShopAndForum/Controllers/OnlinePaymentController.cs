using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace AutoParts_ShopAndForum.Controllers;

public class OnlinePaymentController : Controller
{
    public IActionResult Index()
    {
        //return RedirectPermanent("http://127.0.0.1:4242/checkout.html");

        return View();
    }

    [HttpPost]
    public IActionResult CreateStripeSession()
    {
        var domain = $"{Request.Scheme}://{Request.Host}";
        var successUrl = Url.Action("Success", "Checkout", null, Request.Scheme);
        var cancelUrl = Url.Action("Cancel", "Checkout", null, Request.Scheme);

        var options = new SessionCreateOptions
        {
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
}