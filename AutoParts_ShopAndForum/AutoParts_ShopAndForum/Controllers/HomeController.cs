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
            //var categories = _categoryService.GetAll();

            var categories = new ProductCategoryModel[]
            {
                new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },new ProductCategoryModel()
                {
                    Id = 1,
                    ImageUrl = "https://www.autopower.bg/images/categories/%D0%9C%D0%B0%D1%81%D0%BB%D0%B0%20%D0%B8%20%D1%82%D0%B5%D1%87%D0%BD%D0%BE%D1%81%D1%82%D0%B8.jpg",
                    Name = "Demo" + Guid.NewGuid().ToString().Substring(0, 10),
                },
            };

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
