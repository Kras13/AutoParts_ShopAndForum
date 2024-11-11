using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Models.Product;
using AutoParts_ShopAndForum.Models.ProductSubcategory;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts_ShopAndForum.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductSubcategoryService _subcategoryService;
        public ProductController(
             IProductService productService, IProductSubcategoryService subcategoryService)
        {
            _productService = productService;
            _subcategoryService = subcategoryService;
        }

        public IActionResult All(ProductQueryViewModel model)
        {
            var queryModel = _productService.GetQueried(
               model.CurrentPage,
               model.ProductsPerPage,
               model.SearchCriteria,
               model.Sorting,
               model.CategoryId,
               model.Subcategories?.Where(sb => sb.Selected).Select(sb => sb.Id).ToArray());

            if (model.Subcategories == null)
            {
                model.Subcategories = _subcategoryService
                    .GetAll(model.CategoryId)
                    .Select(sc => new ProductSubcategorySelectViewModel() { Id = sc.Id, Name = sc.Name })
                    .ToArray();
            }

            model.Products = queryModel.Products;
            model.TotalProducts = queryModel.TotalProductsWithoutPagination;

            return View(model);
        }
    }
}
