using AutoParts_ShopAndForum.Areas.Seller.Models;
using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.Product;
using AutoParts_ShopAndForum.Infrastructure;
using AutoParts_ShopAndForum.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
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

            return RedirectToAction("All", "Product", new { area = "" });
        }

        public IActionResult Edit(int productId)
        {
            var product = _productService.GetById(productId);
            var model = new ProductAddInputModel()
            {
                ProductId = productId,
                Subcategories = _categoryService.GetAllSubcategories(),
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                SelectedSubcategoryId = product.SubcategoryId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleType.Administrator)]
        public IActionResult Edit(ProductAddInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model.ProductId);
            }

            _productService.Update(new ProductModel()
            {
                Id = model.ProductId,
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                SubcategoryId = model.SelectedSubcategoryId
            });

            return RedirectToAction("All", "Product", new { area = "" });
        }
    }
}
