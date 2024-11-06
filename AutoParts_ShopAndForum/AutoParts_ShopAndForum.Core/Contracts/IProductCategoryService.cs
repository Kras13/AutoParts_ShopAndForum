using AutoParts_ShopAndForum.Core.Models;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IProductCategoryService
    {
        ProductCategoryModel[] GetAll();
    }
}
