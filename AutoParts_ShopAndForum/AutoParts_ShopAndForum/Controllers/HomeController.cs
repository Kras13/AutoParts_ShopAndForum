using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.ProductCategory;
using AutoParts_ShopAndForum.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AutoParts_ShopAndForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductCategoryService _categoryService;

        public HomeController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var categories = _categoryService.GetAll();

            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
