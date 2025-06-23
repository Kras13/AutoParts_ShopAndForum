using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers;

public class OnlinePaymentController : Controller
{
    public IActionResult Index()
    {
        return RedirectPermanent("http://127.0.0.1:4242/checkout.html");
    }
}