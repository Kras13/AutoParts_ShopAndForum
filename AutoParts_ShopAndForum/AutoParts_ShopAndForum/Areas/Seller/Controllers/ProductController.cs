using AutoParts_ShopAndForum.Areas.Seller.Models;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Areas.Seller.Controllers
{
    public class ProductController : BaseSellerController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;

        public ProductController(
            IProductService productService, IProductCategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Add()
        {
            var model = new ProductAddInputModel()
            {
                Subcategories = _categoryService.GetAllSubcategories()
            };            

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ProductAddInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Subcategories = _categoryService.GetAllSubcategories();

                return View(model);
            }

            _productService.Add(
                 model.Name,
                 model.Price,
                 model.ImageUrl,
                 model.Description,
                 model.SelectedSubcategoryId,
                 this.User.GetId());

            return RedirectToAction("All", "Products", new { area = "" });
        }
    }
}
