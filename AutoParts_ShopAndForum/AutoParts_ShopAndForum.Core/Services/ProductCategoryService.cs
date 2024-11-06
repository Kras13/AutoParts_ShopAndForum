using AutoParts_ShopAndForum.Core.Contracts;
using AutoParts_ShopAndForum.Core.Models;
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
    }
}
