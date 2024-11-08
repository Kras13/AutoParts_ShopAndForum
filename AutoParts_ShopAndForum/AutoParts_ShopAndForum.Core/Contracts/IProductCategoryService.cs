using AutoParts_ShopAndForum.Core.Models.ProductCategory;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IProductCategoryService
    {
        ProductCategoryModel[] GetAll();
    }
}
