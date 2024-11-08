using AutoParts_ShopAndForum.Core.Models.ProductSubcategory;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IProductSubcategoryService
    {
        ProductSubcategoryModel[] GetAll(int? categoryId = null);
    }
}
