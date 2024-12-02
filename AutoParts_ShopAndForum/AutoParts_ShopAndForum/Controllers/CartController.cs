using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Cart;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Models;
using AutoParts_ShopAndForum.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ITownService _townService;
        public CartController(ICartService cartService, ITownService townService)
        {
            _cartService = cartService;
            _townService = townService;
        }

        public IActionResult Add(ProductCartModel model, string lastUrl)
        {
            var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>(CartConstant.Cart);

            _cartService.Add(ref cart, model);

            HttpContext.Session.SetObject(CartConstant.Cart, cart);

            return RedirectToAction("Details", "Product",
                new
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    Price = model.Price,
                    Quantity = model.Quantity,
                    AddedToCart = true,
                    LastUrl = lastUrl
                });
        }

        public IActionResult All()
        {
            var products = HttpContext.Session.GetObject<ICollection<ProductCartModel>>(CartConstant.Cart);

            if (products == null)
                products = new List<ProductCartModel>();

            var model = new CartListViewModel()
            {
                Products = products,
                Towns = _townService.GetAll()
            };

            return View(model);
        }

        public void ChangeProduct(int id, int quantity)
        {
            var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>(CartConstant.Cart);

            _cartService.ChangeQuantity(ref cart, id, quantity);

            HttpContext.Session.SetObject(CartConstant.Cart, cart);
        }

        public void RemoveProduct(int id)
        {
            var cartCollection = HttpContext.Session.GetObject<ICollection<ProductCartModel>>(CartConstant.Cart);
            var selectedModel = cartCollection.FirstOrDefault(m => m.Id == id);

            if (selectedModel == null)
                throw new ArgumentException("Model with such id does not exist in the cart");

            cartCollection.Remove(selectedModel);

            HttpContext.Session.SetObject(CartConstant.Cart, cartCollection);
        }

        [Authorize]
        [HttpPost]
        public void Finalise(string street, int townId)
        {
            var cart = HttpContext.Session.GetObject<ICollection<ProductCartModel>>("Cart");

            if (cart == null || cart.Count == 0)
            {
                throw new InvalidOperationException("Can not finalise an empty Cart...");
            }

            int orderId = _cartService.Order(ref cart, this.User.GetId(), street, townId);

            HttpContext.Session.SetObject(CartConstant.Cart, cart);

            if (orderId > 0)
            {
                TempData["OrderSuccessful"] = 1;
            }
        }
    }
}
