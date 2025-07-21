using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Seller.Controllers;

public class ForecastController : BaseSellerController
{
    public IActionResult Index()
    {
        return View();
    }
    
    
}