using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers;

public class ChatController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}