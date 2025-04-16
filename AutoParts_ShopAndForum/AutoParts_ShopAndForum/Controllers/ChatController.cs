using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers;

[Authorize]
public class ChatController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}