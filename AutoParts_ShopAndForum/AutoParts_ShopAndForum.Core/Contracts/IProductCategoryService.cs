using AutoParts_ShopAndForum.Core.Models.ProductCategory;
using AutoParts_ShopAndForum.Core.Models.ProductSubcategory;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IProductCategoryService
    {
        ProductCategoryModel[] GetAll();

        ProductSubcategoryModel[] GetAllSubcategories();
    }
}
