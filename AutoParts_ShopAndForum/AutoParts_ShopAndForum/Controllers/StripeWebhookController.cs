using AutoParts_ShopAndForum.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AutoParts_ShopAndForum.Controllers;

[IgnoreAntiforgeryToken]
[ApiController]
[Route("api/[controller]")]
public class StripeWebhookController(IConfiguration config, IOrderService orderService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var endpointSecret = config["Stripe:WebhookSecret"];
        var stripeContent = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var signatureHeader = Request.Headers["Stripe-Signature"];
            var stripeEvent = EventUtility.ConstructEvent(stripeContent, signatureHeader, endpointSecret);
            
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                var orderToken = session?.Metadata["orderToken"];

                if (orderToken == null || Guid.TryParse(orderToken, out var orderTokenParsed))
                    return BadRequest();

                _ = orderService.MarkOnlinePaymentAsSuccessful(orderTokenParsed);
            }
            else if (stripeEvent.Type == EventTypes.CheckoutSessionExpired ||
                     stripeEvent.Type == EventTypes.CheckoutSessionAsyncPaymentFailed)
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                var orderToken = session?.Metadata["orderToken"];

                if (orderToken == null || Guid.TryParse(orderToken, out var orderTokenParsed))
                    return BadRequest();

                _ = orderService.MarkOnlinePaymentAsCancelled(orderTokenParsed);
            }

            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest();
        }
    }
}