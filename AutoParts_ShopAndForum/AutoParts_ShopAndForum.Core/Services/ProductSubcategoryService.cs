using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models.ProductSubcategory;
using AutoParts_ShopAndForum.Infrastructure.Data;

namespace AutoParts_ShopAndForum.Core.Services
{
    public class ProductSubcategoryService : IProductSubcategoryService
    {
        private readonly ApplicationDbContext _context;
        public ProductSubcategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProductSubcategoryModel[] GetAll(int? categoryId = null)
        {
            var result = _context.ProductsSubcategories.
                AsQueryable();

            if (categoryId.HasValue)
            {
                result = result.Where(c => c.CategoryId == categoryId);
            }

            return result
                .Select(sc => new ProductSubcategoryModel() { Id = sc.Id, Name = sc.Name })
                .ToArray();
        }
    }
}
