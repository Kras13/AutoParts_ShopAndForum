using AutoParts_ShopAndForum.Core.Models.Product;

namespace AutoParts_ShopAndForum.Core.Contracts
{
    public interface IProductService
    {
        int Add(
            string name, decimal price,
            string imageUrl, string description,
            int subcategoryId, string creatorId);

        ProductQueryModel GetQueried(
            int currentPage,
            int productsPerPage,
            string searchCriteria,
            ProductSorting sorting,
            int? categoryId = null,
            int[] selectedSubcategories = null);

        ProductModel GetById(int id);

        ProductModel Update(ProductModel product);
        ProductModel[] GetAll();
    }
}
