using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.ProductCategory;
using AutoParts_ShopAndForum.Core.Models.ProductSubcategory;
using AutoParts_ShopAndForum.Infrastructure.Data;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProductCategoryModel[] GetAll()
        {
            return _context.ProductsCategories
                .Select(m => new ProductCategoryModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    ImageUrl = m.ImageUrl
                })
                .ToArray();
        }

        public ProductSubcategoryModel[] GetAllSubcategories()
        {
            return _context.ProductsSubcategories
               .Select(m => new ProductSubcategoryModel() { Id = m.Id, Name = m.Name })
               .ToArray();
        }
    }
}
