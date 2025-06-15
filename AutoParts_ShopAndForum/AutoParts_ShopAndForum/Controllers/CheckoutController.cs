using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Core.Models.CourierStation;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Models.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ICourierStationService _courierStationService;
    private readonly ITownService _townService;

    public CheckoutController(
        ICourierStationService courierStationService, ITownService townService)
    {
        _courierStationService = courierStationService;
        _townService = townService;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>("Cart");

        if (cart == null || cart.Count == 0)
        {
            throw new ArgumentException("Cart is empty");
        }

        var towns = _townService.GetAll();
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
        throw new NotImplementedException();
    }

    public ICollection<CourierStationModel> GetCourierStationsForTown([FromRoute] int id) // FromRoute might need to be specified
    {
        return _courierStationService.GetAllByTownId(id);
    }
}